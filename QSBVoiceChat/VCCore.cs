using Adrenak.UniVoice;
using Adrenak.UniVoice.AudioSourceOutput;
using Adrenak.UniVoice.UniMicInput;
using OWML.Common;
using OWML.ModHelper;
using System;
using UnityEngine;

namespace QSBVoiceChat
{
	public class VCCore : ModBehaviour
	{
		public static IModHelper Helper { get; private set; }
		public static IQSBAPI QSBAPI { get; private set; }
		public static ChatroomAgent Agent { get; private set; }
		internal static ChatroomNetwork Network { get; private set; }

		private void Start()
		{
			Helper = ModHelper;
			QSBAPI = ModHelper.Interaction.TryGetModApi<IQSBAPI>("Raicuparta.QuantumSpaceBuddies");
			SetupChatroom();

			QSBAPI.OnPlayerJoin().AddListener(SetupBobbleHead);//Setup Talking Heads for Everyone
		}

		private void SetupBobbleHead(uint playerID)
		{
			Helper.Events.Unity.RunWhen(() => QSBAPI.GetPlayerReady(playerID) && QSBAPI.GetPlayerCamera(playerID) != null, () =>
			{
				TalkingAnimationManager.SetupTalkingHead(playerID);
			});
		}

		private void SetupChatroom()
		{
			Network = new ChatroomNetwork();
			var audioInput = new UniVoiceUniMicInput();
			var audioOutputFactory = new AudioOutputFactory();
			Agent = new ChatroomAgent(Network, audioInput, audioOutputFactory);
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

		public override void Configure(IModConfig config)
		{
			base.Configure(config);
			GetSettingsValueOrDefault<bool>(config, "Player Head Bobbing", true);
		}

		public static T GetSettingsValueOrDefault<T>(IModConfig config, string key, T defaultValue)
		{
			try
			{
				var value = config.GetSettingsValue<T>(key);
				if (value == null)
				{
					config.SetSettingsValue(key, defaultValue);
					return defaultValue;
				}
				if (value is string && ((string)(object)value).Length < 1)
				{
					config.SetSettingsValue(key, defaultValue);
					return defaultValue;
				}
				return value;
			}
			catch (Exception)
			{
				config.SetSettingsValue(key, defaultValue);
				return defaultValue;
			};
		}
	}
}
