// CopyRight FlareHorz Team. All Rights Reserved.

global using static FlareCore.CAssert;
using System;
using System.Diagnostics;

namespace FlareCore;

public class CAssert
{
	/**
	 * Checks a condition and halts execution if it's false.
	 * <param name="bCondition"> The condition to check. </param>
	 * <param name="Message"> Optional message to log when the assertion fails. </param>
	 */
	public static bool Verify(bool bCondition, string Message = "")
	{
		if (bCondition) return true;
		
		CLog.Error($"Assertion failed! {Message}");
		CLog.Error("Stack Trace: \n" + new StackTrace(2));
#if DEBUG
		Debug.Assert(false, Message);
#else
		Environment.Exit(1);
#endif

		return false;
	}

	/** Used for pieces of code which should never be executed. */
	public static bool CheckNoEntry(string Message = "")
	{
		return Verify(false, IsTextValid(Message) ? Message : "This piece of code should never be executed!");
	}
}
