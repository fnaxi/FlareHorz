// CopyRight FlareHorz Team. All Rights Reserved.

using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace FlareCore.Logger;

/// <summary>
/// Logger class.
/// </summary>
[FlareCoreAPI]
public abstract class FLog : FFlareObject
{
	/// <summary>
	/// Log file of this logger.
	/// </summary>
	private static FFile LogFile;
	
	/// <summary>
	/// Log file which saved to Logs/.
	/// </summary>
	private static FFile SavedLogFile;
	
	/// <summary>
	/// Is initialized or not.
	/// </summary>
	private static bool bInitialized = false;
	
	/// <summary>
	/// Setup and initialize logger.
	/// </summary>
	/// <param name="SolutionPath">Where to create the log file.</param>
	public static void Initialize(string SolutionPath, string ProjectName)
	{
#if FH_DEBUG
		string Configuration = "Debug";
#elif FH_DEVELOPMENT
		string Configuration = "Development";
#else
		#error Unknown configuration
#endif

		// There can be no Logs/ folder
		string WorkingDirectory = Path.Combine(SolutionPath, ProjectName, "Logs");
		if (!Directory.Exists(WorkingDirectory))
		{
			Directory.CreateDirectory(WorkingDirectory);
		}
		
		// Create log file
		LogFile = CreateObject<FFile>("LogFile");
		
		string LogFileName = "Log.fhlog";
		string LogFilePath = Path.Combine(WorkingDirectory, LogFileName);
		
		if (File.Exists(LogFilePath))
		{
			LogFile.LoadFile(Path.Combine(WorkingDirectory, LogFileName));
			
			// Seperate logs
			LogFile.WriteSrc("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
		}
		else
		{
			LogFile.CreateFile(WorkingDirectory, LogFileName);
		}

		// Create saved log file
		SavedLogFile = CreateObject<FFile>("SavedLogFile");
		
		DateTime CurrentTime = DateTime.Now;
		string TimeFormatted = CurrentTime.ToString("yyyy-MM-dd_HH-mm-ss");
		string SavedLogFileName = Configuration + "-Log-" + TimeFormatted + ".fhlog";
		string SavedLogFilePath = Path.Combine(WorkingDirectory, SavedLogFileName);
		
		SavedLogFile.CreateFile(WorkingDirectory, SavedLogFileName);
		
		// Initialized now
		bInitialized = true;
	}

	/// <summary>
	/// Shutdown logger.
	/// </summary>
	public static void Shutdown()
	{
		if (!bInitialized) return;
		
		// To make sure Log() will not be called anymore
		bInitialized = false;
		
		// Close log files
		LogFile.Close();
		SavedLogFile.Close();
	}
	
	/// <summary>
	/// Log something to console and log file.
	/// </summary>
	/// <param name="LogVerbosity">Log verbosity to use.</param>
	/// <param name="Text">Text to log.</param>
	public static void Log(ELogVerbosity LogVerbosity, string Text)
	{
		if (!bInitialized || LogVerbosity == ELogVerbosity.All || LogVerbosity == ELogVerbosity.NoLogging) return;
		
		// Change console color and make log verbosity wrapped to 5 symbols
		string LogVerbosityWrapped = "NULL ";
		switch (LogVerbosity)
		{
			case ELogVerbosity.Error:
				Console.ForegroundColor = ConsoleColor.Red;
				LogVerbosityWrapped = "ERROR";
				break;
			case ELogVerbosity.Warning:
				Console.ForegroundColor = ConsoleColor.Yellow;
				LogVerbosityWrapped = "WARN ";
				break;
			case ELogVerbosity.Info:
				Console.ForegroundColor = ConsoleColor.White;
				LogVerbosityWrapped = "INFO ";
				break;
			case ELogVerbosity.Debug:
				Console.ForegroundColor = ConsoleColor.Cyan;
				LogVerbosityWrapped = "DEBUG";
				break;
			default:
				FAssert.CheckNoEntry();
				break;
		}
		
		// Get current time
		DateTime CurrentTime = DateTime.Now;
		
		// Wrap everything
		string TimeFormatted = CurrentTime.ToString("HH:mm:ss");
		string APIFormatted = FReflection.GetAPINameFromStackTrace().PadRight(18);
		string FormattedLogText = "> " + TimeFormatted + "> " + LogVerbosityWrapped + "> " + Text;

		// Log
		Console.WriteLine(FormattedLogText);
		LogFile.WriteSrc(FormattedLogText);
		SavedLogFile.WriteSrc(FormattedLogText);
		
		// Set console color back
		Console.ForegroundColor = ConsoleColor.White;
	}
	
	/// <summary>
	/// Wrappers for different log verbosities.
	/// </summary>
	public static void Error(string Text) { Log(ELogVerbosity.Error, Text); }
	public static void Warn(string Text) { Log(ELogVerbosity.Warning, Text); }
	public static void Info(string Text) { Log(ELogVerbosity.Info, Text); }
	public static void Debug(string Text)
	{
#if FH_DEBUG
		Log(ELogVerbosity.Debug, Text);
#endif
	}
}
