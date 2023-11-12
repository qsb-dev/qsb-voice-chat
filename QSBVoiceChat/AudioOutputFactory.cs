using Adrenak.UniVoice.AudioSourceOutput;
using Adrenak.UniVoice;
using UnityEngine;

namespace QSBVoiceChat;

public class AudioOutputFactory : IAudioOutputFactory
{
	public IAudioOutput Create(int samplingRate, int channelCount, int segmentLength)
	{
		var audioClip = new CircularAudioClip(samplingRate, channelCount, segmentLength, 10);

		var audioSource = new GameObject($"UniVoiceAudioSourceOutput").AddComponent<AudioSource>();
		audioSource.spatialBlend = 1;
		audioSource.rolloffMode = AudioRolloffMode.Linear;
		audioSource.minDistance = 0;
		audioSource.maxDistance = 30;

		return UniVoiceAudioSourceOutput.New(audioClip, audioSource, 5);
	}
}
