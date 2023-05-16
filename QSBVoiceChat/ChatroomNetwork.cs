using Adrenak.UniVoice;
using QSB.Messaging;
using QSB.Player;
using QSB.Utility;
using QSBVoiceChat.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSBVoiceChat;

internal class ChatroomNetwork : IChatroomNetwork
{
	public short OwnID { get; private set; }
	public List<short> PeerIDs { get; }

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
		QSBVoiceChatCore.QSBAPI.GetOnStartHostEvent().AddListener(OnStartHost);
		QSBVoiceChatCore.QSBAPI.GetOnStopHostEvent().AddListener(OnStopHost);
		QSBVoiceChatCore.QSBAPI.GetOnLocalJoinServerEvent().AddListener(OnLocalJoinServer);
		QSBVoiceChatCore.QSBAPI.GetOnLocalLeaveServerEvent().AddListener(OnLocalLeaveServer);
		QSBVoiceChatCore.QSBAPI.GetOnPeerJoinServerEvent().AddListener(OnPeerJoinServer);
		QSBVoiceChatCore.QSBAPI.GetOnPeerLeaveServerEvent().AddListener(OnPeerLeaveServer);

		PeerIDs = new();
	}

	// we're just using piggybacking off QSB's networking, dont need these
	public void HostChatroom(object data = null) => throw new NotImplementedException();
	public void JoinChatroom(object data = null) => throw new NotImplementedException();
	public void LeaveChatroom(object data = null) => throw new NotImplementedException();
	public void CloseChatroom(object data = null) => throw new NotImplementedException();

	public void Dispose()
	{
		QSBVoiceChatCore.QSBAPI.GetOnStartHostEvent().RemoveListener(OnStartHost);
		QSBVoiceChatCore.QSBAPI.GetOnStopHostEvent().RemoveListener(OnStopHost);
		QSBVoiceChatCore.QSBAPI.GetOnLocalJoinServerEvent().RemoveListener(OnLocalJoinServer);
		QSBVoiceChatCore.QSBAPI.GetOnLocalLeaveServerEvent().RemoveListener(OnLocalLeaveServer);
		QSBVoiceChatCore.QSBAPI.GetOnPeerJoinServerEvent().RemoveListener(OnPeerJoinServer);
		QSBVoiceChatCore.QSBAPI.GetOnPeerLeaveServerEvent().RemoveListener(OnPeerLeaveServer);
	}

	public void SendAudioSegment(short peerID, ChatroomAudioSegment data)
	{
		new AudioSegmentMessage(peerID, data).Send();
		OnAudioSent?.SafeInvoke(peerID, data);
	}

	public void GotAudioMessage(short peerId, ChatroomAudioSegment data)
	{
		OnAudioReceived?.SafeInvoke(peerId, data);
	}

	private void OnStartHost()
	{
		DebugLog.ToConsole($"ON START HOST", OWML.Common.MessageType.Info);
		OnCreatedChatroom?.SafeInvoke();
	}
	
	private void OnStopHost()
	{
		DebugLog.ToConsole($"ON STOP HOST", OWML.Common.MessageType.Info);
		OnClosedChatroom?.SafeInvoke();
	}

	private void OnLocalJoinServer(uint id)
	{
		DebugLog.ToConsole($"ON LOCAL JOIN SERVER", OWML.Common.MessageType.Info);
		OnJoinedChatroom?.SafeInvoke((short)id);
		OwnID = (short)id;

		var alreadyConnectedPlayers = QSBPlayerManager.PlayerList.Where(x => !x.IsLocalPlayer).ToList();
		foreach (var item in alreadyConnectedPlayers)
		{
			PeerIDs.Add((short)item.PlayerId);
			OnPeerJoinedChatroom?.SafeInvoke((short)item.PlayerId);
		}
	}

	private void OnLocalLeaveServer()
	{
		DebugLog.ToConsole($"ON LOCAL LEAVE SERVER", OWML.Common.MessageType.Info);
		OnLeftChatroom?.SafeInvoke();

		foreach (var item in PeerIDs)
		{
			OnPeerLeftChatroom?.SafeInvoke(item);
		}

		PeerIDs.Clear();
	}

	private void OnPeerJoinServer(uint id)
	{
		DebugLog.ToConsole($"ON PEER JOIN SERVER", OWML.Common.MessageType.Info);
		OnPeerJoinedChatroom?.SafeInvoke((short)id);
		PeerIDs.Add((short)id);
	}

	private void OnPeerLeaveServer(uint id)
	{
		DebugLog.ToConsole($"ON PEER LEFT SERVER", OWML.Common.MessageType.Info);
		OnPeerLeftChatroom?.SafeInvoke((short)id);
		PeerIDs.Remove((short)id);
	}
}
