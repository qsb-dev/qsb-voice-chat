using System;
using System.Collections.Generic;
using OWML.ModHelper;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace QSBVoiceChat;

// Adapted from https://github.com/BUNN1E5/DiscordProximityChat/blob/master/DiscordProximityChat/TalkingAnimationController.cs

public static class TalkingAnimationManager
{
	public static void SetupTalkingHead(uint id)
	{
		var body = VCCore.QSBAPI.GetPlayerBody(id);
		if (body == null)
			return;

		var isLocalPlayer = id == VCCore.QSBAPI.GetLocalPlayerID();

		var root = body.transform.Find(isLocalPlayer
			? "Traveller_HEA_Player_v2"
			: "REMOTE_Traveller_HEA_Player_v2"
		);

		if (root == null)
			return;

		var playerHead = root.Find(
			"Traveller_Rig_v01:Traveller_Trajectory_Jnt/Traveller_Rig_v01:Traveller_ROOT_Jnt/Traveller_Rig_v01:Traveller_Spine_01_Jnt/Traveller_Rig_v01:Traveller_Spine_02_Jnt/Traveller_Rig_v01:Traveller_Spine_Top_Jnt/Traveller_Rig_v01:Traveller_Neck_01_Jnt/Traveller_Rig_v01:Traveller_Neck_Top_Jnt"
		);

		if (playerHead == null)
			return;

		VCCore.Helper.Console.WriteLine(
			$"Everything seems OK {(isLocalPlayer ? "local" : "remote")}"
		);

		Create(playerHead, id);
	}

	public static TalkingAnimationController Create(
		Transform transform,
		uint id
	)
	{
		var existingComponent = transform.GetComponent<TalkingAnimationController>();
		existingComponent =
			existingComponent != null
				? existingComponent
				: transform.gameObject.AddComponent<TalkingAnimationController>();
		existingComponent.playerID = (short)id;
		return existingComponent;
	}
}

public class TalkingAnimationController : MonoBehaviour
{
	public float animationSpeed = 25;
	public float animationAmplitude = 0.005f;
	public short playerID;

	private void Update()
	{
		if (!VCCore.Network.IsSpeaking.ContainsKey(playerID))
		{
			transform.localScale = Vector3.one;
			return;
		}

		if (VCCore.Network.IsSpeaking[playerID])
		{
			var animationTime = Time.time * animationSpeed;
			var multiplier = animationSpeed * animationAmplitude;
			var offset = 1 - animationAmplitude * 0.5f;
			var x = Mathf.Sin(animationTime);
			var z = Mathf.Cos(animationTime);
			if (VCCore.Helper.Config.GetSettingsValue<bool>("Player Head Bobbing"))
			{
				transform.localScale = new Vector3(
					x * multiplier + offset,
					1,
					z * multiplier + offset
				);
			}
		}
		else
		{
			transform.localScale = Vector3.one;
		}
	}
}
