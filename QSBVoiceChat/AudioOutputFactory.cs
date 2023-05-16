using Adrenak.UniVoice;
using Adrenak.UniVoice.AudioSourceOutput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace QSBVoiceChat;

internal class AudioOutputFactory : IAudioOutputFactory
{
	public IAudioOutput Create(int frequency, int channelCount, int samplesLen)
	{
		var audioSourceGO = new GameObject();
		var audioSource = audioSourceGO.AddComponent<AudioSource>();
		audioSource.minDistance = 0;
		audioSource.maxDistance = 10;
		// i have no idea what im doing with these values lol
		return UniVoiceAudioSourceOutput.New(new CircularAudioClip(16000, 1, 1600, 25), audioSource, 5);
	}
}
