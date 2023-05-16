using UnityEngine.Events;

namespace QSBVoiceChat;
public interface IQSBAPI
{
	public UnityEvent GetOnStartHostEvent();
	public UnityEvent GetOnStopHostEvent();
	public UnityEvent<uint> GetOnLocalJoinServerEvent();
	public UnityEvent GetOnLocalLeaveServerEvent();
	public UnityEvent<uint> GetOnPeerJoinServerEvent();
	public UnityEvent<uint> GetOnPeerLeaveServerEvent();
}
