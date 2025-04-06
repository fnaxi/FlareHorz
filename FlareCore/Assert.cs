// CopyRight FlareHorz Team. All Rights Reserved.

using System;
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
				FLog.Warn("Verify assertion failed!");
				LogAssertInfo(Message, bVerify);
				break;
			case false:
				FLog.Error("Assertion failed!");
				LogAssertInfo(Message, bVerify);
				Environment.Exit(1);
				break;
		}
	}

	/// <summary>
	/// Log info about current assert.
	/// </summary>
	private static void LogAssertInfo(string InMessage, bool bVerify)
	{
		ELogVerbosity LV = ELogVerbosity.Error;
		if (bVerify)
		{
			LV = ELogVerbosity.Warning;
		}
		
		// Get stack info
		StackFrame StackInstance = new StackFrame(3, true); // Skip LogAssertInfo(), Assert() and Verify()/Check()/etc.
		
		// Get info
		MethodBase EpxrMethod = StackInstance.GetMethod();
		string ExprFileName = StackInstance.GetFileName();
		int EpxrLineNumber = StackInstance.GetFileLineNumber();
		
		// Log everything
		FLog.Log(LV, EpxrMethod.ToString());
		FLog.Log(LV, "In " + ExprFileName + " on " + EpxrLineNumber + " line");
		if (InMessage != string.Empty)
		{
			FLog.Log(LV, "Message: " + InMessage);
		}

		// Shutdown logger
		if (!bVerify)
		{
			FLog.Shutdown();
		}
	}

	/// <summary>
	/// Checks condition, if it's false - crash application and log details.
	/// Works in both Debug and Development.
	/// <returns>bCondition.</returns>
	/// </summary>
	public static bool Check(bool bCondition)
	{
		Assert(bCondition);
		return bCondition;
	}

	public static bool Checkf(bool bCondition, string Message)
	{
		Assert(bCondition, Message);
		return bCondition;
	}

	/// <summary>
	/// Checks condition, if it's false - log warning but does not crash application.
	/// Works in both Debug and Development.
	/// <returns>bCondition.</returns>
	/// </summary>
	public static bool Verify(bool bCondition)
	{
		Assert(bCondition, "", true);
		return bCondition;
	}
	public static bool Verifyf(bool bCondition, string Message) 
	{ 
		Assert(bCondition, Message, true);
		return bCondition; 
	}
	
	/// <summary>
	/// Same as Check() but works only Debug, intended for something that is slow to check.
	/// <returns>bCondition.</returns>
	/// </summary>
	public static bool CheckSlow(bool bCondition)
	{
#if FH_DEBUG
		Assert(bCondition);
		return bCondition;
#else
		return false;
#endif
	}
	public static bool CheckfSlow(bool bCondition, string Message)
	{
#if FH_DEBUG
		Assert(bCondition, Message);
		return bCondition;
#else
		return false;
#endif
	}
	
	/// <summary>
	/// Should be used for pieces of code which should never be executed.
	/// <returns>bCondition.</returns>
	/// </summary>
	public static bool CheckNoEntry() { return Check(false); }
}

/// <summary>
/// Reentry guard assert.
/// </summary>
[FlareCoreAPI]
public class FReentryGuard : FFlareObject
{
	/// <summary>
	/// Calls when object of that class is created.
	/// </summary>
	protected override void OnObjectCreated()
	{ 
		base.OnObjectCreated();

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
