// CopyRight FlareHorz Team. All Rights Reserved.

using System;
using FlareCore.Logger;

namespace FlareCore;

/// <summary>
/// Action time. Check performance of specific action.
/// </summary>
[FlareCoreAPI]
public class FActionTime : FFlareObject
{
	/// <summary>
	/// Calls when object of that class is created.
	/// </summary>
	protected override void OnObjectCreated()
	{ 
		base.OnObjectCreated();
		
		TimeStart = new DateTime();
		TimeEnd = new DateTime();
		FinalTime = new TimeSpan();
		
		RG = CreateObject<FReentryGuard>("RG");
		
		StartInternal();
		FLog.Debug(GetName() + " was started");
	}
	
	/// <summary>
	/// Start time of this action.
	/// </summary>
	private DateTime TimeStart;
	
	/// <summary>
	/// End time of this action.
	/// </summary>
	private DateTime TimeEnd;

	/// <summary>
	/// Final time.
	/// </summary>
	private TimeSpan FinalTime;

	/// <summary>
	/// Reentry guard.
	/// </summary>
	private FReentryGuard RG;
	
	/// <summary>
	/// Create object of set type. Can be used only with FFlareObject derived classes.
	/// </summary>
	public static FActionTime Start(string InName)
	{
		FActionTime Instance = CreateObject<FActionTime>(InName);
		return Instance; 
	}
	
	/// <summary>
	/// Start action.
	/// </summary>
	private void StartInternal()
	{
		TimeStart = DateTime.Now;
	}

	/// <summary>
	/// Stop action.
	/// </summary>
	public void Stop()
	{
		RG.CheckNoReentry();
		
		TimeEnd = DateTime.Now;
		FinalTime = TimeEnd - TimeStart;
		
		FLog.Debug(GetName() + " finished with " + GetTimeSeconds() + "s elapsed");
	}

	/// <summary>
	/// Get final time in string.
	/// </summary>
	public string GetTimeStringFormatted()
	{
		return (FinalTime.Hours + "h " + FinalTime.Minutes + "m " + FinalTime.Seconds + "s ");
	}

	/// <summary>
	/// Get final time.
	/// </summary>
	public float GetTimeSeconds()
	{
		return (float)Math.Round(FinalTime.TotalSeconds + FinalTime.Milliseconds / 1000.0, 2);
	}
}
