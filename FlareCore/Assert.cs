// CopyRight FlareHorz Engine Development Team. All Rights Reserved.

using System.Diagnostics;
using System.Reflection;
using FlareCore.Logger;

namespace FlareCore;

/// <summary>
/// Asserts.
/// </summary>
[FlareCoreAPI]
public class FAssert : FFlareObject
{
	/// <summary>
	/// Assert implementation.
	/// </summary>
	private static void Assert(bool bCondition, string Message = "", bool bVerify = false)
	{
		if (bCondition) return;
		
		// Log everything
		switch (bVerify)
		{
			case true:
				FLog.Error("Verify assertion failed!");
				LogAssertInfo(Message);
				break;
			case false:
				FLog.Error("Assertion failed!");
				LogAssertInfo(Message);
				Environment.Exit(1);
				break;
		}
	}

	/// <summary>
	/// Log info about current assert.
	/// </summary>
	private static void LogAssertInfo(string InMessage)
	{
		// Get stack info
		StackFrame StackInstance = new StackFrame(3, true); // Skip LogAssertInfo(), Assert() and Verify()/Check()/etc.
		
		// Get info
		MethodBase? EpxrMethod = StackInstance.GetMethod();
		string? ExprFileName = StackInstance.GetFileName();
		int EpxrLineNumber = StackInstance.GetFileLineNumber();
		
		// Log everything
		FLog.Error(EpxrMethod + ":" + " in " + ExprFileName + " on " + EpxrLineNumber + " line");
		if (InMessage != string.Empty)
		{
			FLog.Error("Message: " + InMessage);
		}

		// Shutdown logger
		FLog.Shutdown();
	}
	
	/// <summary>
	/// Checks condition, if it's false - crash application and log details.
	/// Works in both Debug and Development.
	/// </summary>
	public static void Check(bool bCondition) { Assert(bCondition); }
	public static void Checkf(bool bCondition, string Message) { Assert(bCondition, Message); }
	
	/// <summary>
	/// Checks condition, if it's false - log error, does not crash application.
	/// Works in both Debug and Development.
	/// </summary>
	public static void Verify(bool bCondition) { Assert(bCondition, "", true); }
	public static void Verifyf(bool bCondition, string Message) { Assert(bCondition, Message, true); }
	
	/// <summary>
	/// Same as Check() but works only Debug, intended for something that is slow.
	/// </summary>
	public static void CheckSlow(bool bCondition)
	{
#if DEBUG
		Assert(bCondition);
#endif
	}
	public static void CheckfSlow(bool bCondition, string Message)
	{
#if DEBUG
		Assert(bCondition, Message);
#endif
	}
	
	/// <summary>
	/// Should be used for pieces of code which should never be executed.
	/// </summary>
	public static void CheckNoEntry() { Check(false); }
}

/// <summary>
/// Reentry guard assert.
/// </summary>
[FlareCoreAPI]
public class FReentryGuard : FFlareObject
{
	/// <summary>
	/// Sets default values.
	/// </summary>
	public FReentryGuard()
	{
		bIsReentered = false;
	}

	/// <summary>
	/// Halts execution if called more than once.
	/// </summary>
	public void CheckNoReentry()
	{
		// Check if the function is already entered
		if (bIsReentered)
		{
			FAssert.CheckNoEntry();
		}
		
		// Mark the method as entered for this instance
		bIsReentered = true;
	}

	/// <summary>
	/// Flag to track whether the method is already entered for this specific instance.
	/// </summary>
	private bool bIsReentered;
}
