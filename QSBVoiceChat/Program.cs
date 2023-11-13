using Adrenak.UniVoice;
using Adrenak.UniVoice.AudioSourceOutput;
using Adrenak.UniVoice.UniMicInput;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

namespace QSBVoiceChat
{
	public class VCCore : ModBehaviour
	{
		public static IModHelper Helper { get; private set; }
		public static IQSBAPI QSBAPI { get; private set; }
		public static ChatroomAgent Agent { get; private set; }

		private void Start()
		{
			Helper = ModHelper;
			QSBAPI = ModHelper.Interaction.TryGetModApi<IQSBAPI>("Raicuparta.QuantumSpaceBuddies");
			SetupChatroom();
		}

		private void SetupChatroom()
		{
			var network = new ChatroomNetwork();
			var audioInput = new UniVoiceUniMicInput();
			var audioOutputFactory = new AudioOutputFactory();
			Agent = new ChatroomAgent(network, audioInput, audioOutputFactory);
		}

		private void Update()
		{
			foreach (var item in Agent.PeerOutputs)
			{
				var playerid = (uint)item.Key;

				if (!QSBAPI.GetPlayerReady(playerid))
				{
					continue;
				}

				var output = item.Value as UniVoiceAudioSourceOutput;

				var playerCamera = QSBAPI.GetPlayerCamera(playerid);
				output.transform.parent = playerCamera.transform;
				output.transform.localPosition = Vector3.zero;
			}
		}
	}
}
