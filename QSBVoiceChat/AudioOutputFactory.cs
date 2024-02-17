﻿using Adrenak.UniVoice.AudioSourceOutput;
using Adrenak.UniVoice;
using UnityEngine;

namespace QSBVoiceChat;

public class AudioOutputFactory : IAudioOutputFactory
{
	// copied from UniVoiceAudioSourceOutput.Factory
	public IAudioOutput Create(int samplingRate, int channelCount, int segmentLength)
	{
		var audioClip = new CircularAudioClip(samplingRate, channelCount, segmentLength, 10);

		var audioSource = new GameObject($"UniVoiceAudioSourceOutput").AddComponent<AudioSource>();
		audioSource.spatialBlend = 1;
		audioSource.rolloffMode = AudioRolloffMode.Linear;
		audioSource.minDistance = 5;
		audioSource.maxDistance = 50;

		audioSource.gameObject.SetActive(false);

		var owAudioSource = audioSource.gameObject.AddComponent<OWAudioSource>();
		owAudioSource._track = OWAudioMixer.TrackName.Player_External;
		owAudioSource._audioLibraryClip = AudioType.None;

		audioSource.gameObject.SetActive(true);

		return UniVoiceAudioSourceOutput.New(audioClip, audioSource, 5);
	}
}
