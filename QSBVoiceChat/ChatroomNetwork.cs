﻿using Adrenak.UniVoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace QSBVoiceChat;

internal class ChatroomNetwork : IChatroomNetwork
{
	public short OwnID { get; private set; }
	public List<short> PeerIDs { get; }
	public Dictionary<long, bool> IsSpeaking { get; }

	public event Action OnCreatedChatroom; //
	public event Action<Exception> OnChatroomCreationFailed;
	public event Action OnClosedChatroom; //
	public event Action<short> OnJoinedChatroom; //
	public event Action<Exception> OnChatroomJoinFailed;
	public event Action OnLeftChatroom; //
	public event Action<short> OnPeerJoinedChatroom; //
	public event Action<short> OnPeerLeftChatroom; //
	public event Action<short, ChatroomAudioSegment> OnAudioReceived;
	public event Action<short, ChatroomAudioSegment> OnAudioSent;

	public ChatroomNetwork()
	{
		VCCore.QSBAPI.OnPlayerJoin().AddListener(OnPlayerJoin);
		VCCore.QSBAPI.OnPlayerLeave().AddListener(OnPlayerLeave);

		VCCore.QSBAPI.RegisterHandler<ChatroomAudioSegment>("QSBVoiceChat-AudioMessage", (peerId, data) => GotAudioMessage((short)peerId, data));

		PeerIDs = new();
		IsSpeaking = new();
	}

	// we're just using piggybacking off QSB's networking, dont need these
	public void HostChatroom(object data = null) => throw new NotImplementedException();
	public void JoinChatroom(object data = null) => throw new NotImplementedException();
	public void LeaveChatroom(object data = null) => throw new NotImplementedException();
	public void CloseChatroom(object data = null) => throw new NotImplementedException();

	public void Dispose()
	{
		VCCore.QSBAPI.OnPlayerJoin().RemoveListener(OnPlayerJoin);
		VCCore.QSBAPI.OnPlayerLeave().RemoveListener(OnPlayerLeave);
	}

	public void OnPlayerJoin(uint id)
	{
		if (id == VCCore.QSBAPI.GetLocalPlayerID())
		{
			if (VCCore.QSBAPI.GetIsHost())
			{
				OnStartHost();
				return;
			}

			OnLocalJoinServer();
		}
		else
		{
			OnPeerJoinServer(id);
		}
	}

	public void OnPlayerLeave(uint id)
	{
		if (id == VCCore.QSBAPI.GetLocalPlayerID())
		{
			if (VCCore.QSBAPI.GetIsHost())
			{
				OnStopHost();
			}

			OnLocalLeaveServer();
		}
		else
		{
			OnPeerLeaveServer(id);
		}
	}

	public void SendAudioSegment(short peerID, ChatroomAudioSegment data)
	{
		VCCore.QSBAPI.SendMessage("QSBVoiceChat-AudioMessage", data, (uint)peerID);
		OnAudioSent?.SafeInvoke(peerID, data);
	}

	public static readonly int minimumDecibels = -60;

	public void GotAudioMessage(short peerId, ChatroomAudioSegment data)
	{
		float levelMax = 0;
		for (var i = 0; i < data.samples.Length; i++)
		{
			data.samples[i] *= 2;
			data.samples[i] = Mathf.Clamp(data.samples[i], -1, 1);
			float wavePeak = data.samples[i] * data.samples[i];
			if (levelMax < wavePeak) levelMax = wavePeak;
		}
		float db = 20 * Mathf.Log10(Mathf.Abs(levelMax));
		IsSpeaking[peerId] = db > minimumDecibels;

		OnAudioReceived?.SafeInvoke(peerId, data);
	}

	private void OnStartHost()
	{
		VCCore.Helper.Console.WriteLine($"ON START HOST", OWML.Common.MessageType.Info);
		OnCreatedChatroom?.SafeInvoke();
	}
	
	private void OnStopHost()
	{
		VCCore.Helper.Console.WriteLine($"ON STOP HOST", OWML.Common.MessageType.Info);
		OnClosedChatroom?.SafeInvoke();
	}

	private void OnLocalJoinServer()
	{
		var id = VCCore.QSBAPI.GetLocalPlayerID();
		VCCore.Helper.Console.WriteLine($"ON LOCAL JOIN SERVER", OWML.Common.MessageType.Info);
		OnJoinedChatroom?.SafeInvoke((short)id);
		OwnID = (short)id;

		var alreadyConnectedPlayers = VCCore.QSBAPI.GetPlayerIDs().Where(x => x != VCCore.QSBAPI.GetLocalPlayerID());
		foreach (var item in alreadyConnectedPlayers)
		{
			PeerIDs.Add((short)item);
			OnPeerJoinedChatroom?.SafeInvoke((short)item);
		}
	}

	private void OnLocalLeaveServer()
	{
		VCCore.Helper.Console.WriteLine($"ON LOCAL LEAVE SERVER", OWML.Common.MessageType.Info);
		OnLeftChatroom?.SafeInvoke();

		foreach (var item in PeerIDs)
		{
			OnPeerLeftChatroom?.SafeInvoke(item);
		}

		PeerIDs.Clear();
	}

	private void OnPeerJoinServer(uint id)
	{
		VCCore.Helper.Console.WriteLine($"ON PEER JOIN SERVER", OWML.Common.MessageType.Info);
		OnPeerJoinedChatroom?.SafeInvoke((short)id);
		PeerIDs.Add((short)id);
	}

	private void OnPeerLeaveServer(uint id)
	{
		VCCore.Helper.Console.WriteLine($"ON PEER LEFT SERVER", OWML.Common.MessageType.Info);
		OnPeerLeftChatroom?.SafeInvoke((short)id);
		PeerIDs.Remove((short)id);
	}
}
