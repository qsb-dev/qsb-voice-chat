using Adrenak.UniVoice;
using Adrenak.UniVoice.AudioSourceOutput;
using Adrenak.UniVoice.UniMicInput;
using OWML.Common;
using OWML.ModHelper;
using System.Linq;

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
			var audioOutputFactory = new UniVoiceAudioSourceOutput.Factory();
			Agent = new ChatroomAgent(network, audioInput, audioOutputFactory);
		}

		private void Update()
		{
			if (Locator.GetPlayerCamera() == null)
			{
				return;
			}

			foreach (var item in Agent.PeerOutputs.Values.Cast<UniVoiceAudioSourceOutput>())
			{
				item.transform.position = Locator.GetPlayerCamera().transform.position;
			}
		}
	}
}
