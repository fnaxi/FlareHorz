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
	/// Sets default values.
	/// </summary>
	private FAssert()
	{
	}

	/// <summary>
	/// Assert implementation.
	/// </summary>
	private static void Assert(bool bCondition, string Message = "", bool bVerify = false)
	{
		if (bCondition) return;
		
		// Get stack info
		StackFrame StackInstance = new StackFrame(2, true); // 2 frames - Assert() and Verify()/Check()/etc.
		
		// Log everything
		if (bVerify)
		{
			FLog.Error("Verify ssertion failed!");
			LogAssertInfo(StackInstance, Message);
		}
		else
		{
			FLog.Error("Assertion failed!");
			LogAssertInfo(StackInstance, Message);
			
			// TODO: ...
			// throw new InvalidOperationException();
			Environment.Exit(1);
		}
	}

	/// <summary>
	/// Log info about current assert.
	/// </summary>
	/// <param name="InStackInstance"></param>
	private static void LogAssertInfo(StackFrame InStackInstance, string InMessage)
	{
		// Get info
		MethodBase? EpxrMethod = InStackInstance.GetMethod();
		string? ExprFileName = InStackInstance.GetFileName();
		int EpxrLineNumber = InStackInstance.GetFileLineNumber();
		
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
	public static void Check(bool bCondition)
	{
		Assert(bCondition);
	}
	public static void Checkf(bool bCondition, string Message)
	{
		Assert(bCondition, Message);
	}
	
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
	/// Checks condition, if it's false - log warning, does not crash application.
	/// Works in both Debug and Development.
	/// </summary>
	public static void Verify(bool bCondition)
	{
		Assert(bCondition, "", true);
	}
	public static void Verifyf(bool bCondition, string Message)
	{
		Assert(bCondition, Message, true);
	}
	
	/// <summary>
	/// Should be used for pieces of code which should be never run.
	/// </summary>
	public static void CheckNoEntry()
	{
		Check(false);
	}
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
