using Adrenak.UniVoice;
using Mirror;
using QSB.Messaging;

namespace QSBVoiceChat.Messages;

internal class AudioSegmentMessage : QSBMessage
{
	private short PeerID;
	private ChatroomAudioSegment Data;

	public AudioSegmentMessage(short peerID, ChatroomAudioSegment data)
	{
		PeerID = peerID;
		Data = data;
	}

	public override void Serialize(NetworkWriter writer)
	{
		base.Serialize(writer);
		writer.Write(PeerID);
		writer.Write(Data.segmentIndex);
		writer.Write(Data.frequency);
		writer.Write(Data.channelCount);
		writer.Write(Data.samples);
	}

	public override void Deserialize(NetworkReader reader)
	{
		base.Deserialize(reader);
		PeerID = reader.ReadShort();

		Data = new ChatroomAudioSegment
		{
			segmentIndex = reader.ReadInt(),
			frequency = reader.ReadInt(),
			channelCount = reader.ReadInt(),
			samples = reader.Read<float[]>()
		};
	}

	public override void OnReceiveRemote()
	{
		(QSBVoiceChatCore.Agent.Network as ChatroomNetwork).GotAudioMessage(PeerID, Data);
	}
}
