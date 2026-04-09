// CopyRight FlareHorz Team. All Rights Reserved.

using System;

namespace FlareCore;

public class CAction
{
	private CAction()
	{
		TimeStart = DateTime.Now;
		TimeEnd = new DateTime();
		FinalTime = TimeSpan.Zero;
	}

	public static void Run(bool bLogTime, Func<string> Lambda)
	{
		CAction TimeAction = new CAction();
		
		string FinishMessage = Lambda();
		TimeAction.Finish(FinishMessage, bLogTime);
	}

	public override string ToString()
	{
		return Convert.ToString( Math.Round(FinalTime.TotalSeconds + FinalTime.Milliseconds / 1000.0, 2) );
	}
	
	private readonly DateTime TimeStart;
	private DateTime TimeEnd;
	private TimeSpan FinalTime;
	
	private void Finish(string Message, bool bLogTime)
	{
		TimeEnd = DateTime.Now;
		FinalTime = TimeEnd - TimeStart;

		if (IsTextValid(Message))
		{
			CLog.Info(bLogTime ? $"{Message} ({ToString()}s elapsed)" : Message);
		}
	}
}
