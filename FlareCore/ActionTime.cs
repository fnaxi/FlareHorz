// CopyRight FlareHorz Team. All Rights Reserved.

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
	/// Start action.
	/// </summary>
	public void Start()
	{
		TimeStart = DateTime.Now;
	}

	/// <summary>
	/// Stop action.
	/// </summary>
	public void Stop()
	{
		TimeEnd = DateTime.Now;

		FinalTime = TimeEnd - TimeStart;
	}

	/// <summary>
	/// Get final time in string.
	/// </summary>
	public string GetTimeStringFormatted()
	{
		return (FinalTime.Hours + "h " + FinalTime.Minutes + "m " + FinalTime.Seconds + "s ");
	}
	
	/// <summary>
	/// Get final time in string.
	/// </summary>
	public string GetTimeStringSeconds()
	{
		return GetTimeSeconds().ToString();
	}

	/// <summary>
	/// Get final time.
	/// </summary>
	public double GetTimeSeconds()
	{
		return FinalTime.TotalSeconds;
	}
}
