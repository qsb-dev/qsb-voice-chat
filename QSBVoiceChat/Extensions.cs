using System;
using System.Reflection;
using OWML.Common;

namespace QSBVoiceChat;

public static class Extensions
{
	// copied from qsb
	public static void SafeInvoke(this MulticastDelegate multicast, params object[] args)
	{
		foreach (var del in multicast.GetInvocationList())
		{
			try
			{
				del.DynamicInvoke(args);
			}
			catch (TargetInvocationException ex)
			{
				VCCore.Helper.Console.WriteLine($"Error invoking delegate! {ex.InnerException}", MessageType.Error);
			}
		}
	}
}
