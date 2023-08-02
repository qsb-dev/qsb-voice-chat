using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adrenak.UniVoice;
using NAudio.Wave;

namespace QSBVoiceChat;

internal class AudioFileAudioInput : IAudioInput
{
	public event Action<int, float[]> OnSegmentReady;

	public int Frequency => 8000;

	public int ChannelCount => 1;

	public int SegmentRate => 10;

	public AudioFileAudioInput()
	{
		var file = @"C:\Users\Henry\Downloads\youve-got-mail-sound\youve-got-mail-sound.mp3";
		var audioFile = new AudioFileReader(file);
		var output = new WaveOutEvent();
		output.Init(audioFile);
		output.Play();
	}

	public void Dispose()
	{

	}
}
