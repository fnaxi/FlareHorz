// CopyRight FlareHorz Team. All Rights Reserved.

using System;

namespace FlareCore;

/**
 * An object that records time on its creation and subtracts it with time recorded when Stop() method was called.
 * NOTE: There's already System.Timers.Timer class, but it uses UpperCamelCase without prefix so we can't declare a variable with name 'Timer'.
 */
public class CTimer
{
	public CTimer()
	{
		StartTime = DateTime.Now;
	}
	public void Stop()
	{
		EndTime = DateTime.Now;
		FinalTime = EndTime - StartTime;
	}

	public override string ToString()
	{
		return Convert.ToString( Math.Round(FinalTime.TotalSeconds + FinalTime.Milliseconds, 0) ) + "ms";
	}
	
	private readonly DateTime StartTime;
	private DateTime EndTime;
	private TimeSpan FinalTime;
}

public class CAction
{
	private CAction()
	{
		Timer = new CTimer();
	}

	public static void Run(bool bLogTime, Func<string> Lambda)
	{
		CAction Action = new CAction();
		
		string FinishMessage = Lambda();
		Action.Finish(FinishMessage, bLogTime);
	}

	public override string ToString()
	{
		return Timer.ToString();
	}

	private readonly CTimer Timer;
	
	private void Finish(string Message, bool bLogTime)
	{
		Timer.Stop();

		if (IsTextValid(Message))
		{
			CLog.Info(bLogTime ? $"{Message} ({ToString()} elapsed)" : Message);
		}
	}
}
