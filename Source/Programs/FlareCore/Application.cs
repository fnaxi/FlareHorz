// CopyRight FlareHorz Team. All Rights Reserved.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace FlareCore;

public class CApplication
{
	/**
	 * Entry-point of the application.
	 * <param name="Arguments"> Command-line arguments the application was run with. </param>
	 * <param name="TargetAssembly"> Pass info about executable assembly to parse metadata such as CmdParam or CmdFlag attributes. </param>
	 */
	public Int32 Run(string[] Arguments, Assembly TargetAssembly)
	{
		Initialize(Arguments, TargetAssembly);
		
		Int32 ExitResult = GuardedMain();
		CLog.Status(ExitResult, $"Exited with code {ExitResult}");

		Shutdown();
		return ExitResult;
	}
	
	/**
	 * Core method where the main application logic should be implemented.
	 * <returns> Exit result code. If it's not 0, log an error. </returns>
	 */
	protected virtual Int32 GuardedMain()
	{
		return Convert.ToInt32( CheckNoEntry("GuardedMain() method must be implemented!") );
	}
	
	/** Executes a console command or some executable. */
	protected static Int32 ExecuteConsoleCommand(string InCommand)
	{
		Verify(IsTextValid(InCommand));
		CLog.Info($"Running command: {InCommand}");

		ProcessStartInfo CommandProcessInfo = new ProcessStartInfo("cmd.exe", "/c" + InCommand)
		{
			CreateNoWindow = true,
			UseShellExecute = false,
			RedirectStandardError = true,
			RedirectStandardOutput = true
		};

		Process CommandProcess = Process.Start(CommandProcessInfo);
		Verify(CommandProcess != null, $"Process is null. Can't execute command {InCommand}");
		
		// Log output
		CommandProcess.OutputDataReceived += (_, e) =>
		{
			if (IsTextValid(e.Data)) CLog.Info($"Output: {e.Data}");
		};
		CommandProcess.BeginOutputReadLine();

		// Log any errors
		CommandProcess.ErrorDataReceived += (_, e) =>
		{
			if (IsTextValid(e.Data)) CLog.Error($"Error: {e.Data}");
		};
		CommandProcess.BeginErrorReadLine();
		
		CommandProcess.WaitForExit();
		Int32 ExitCode = CommandProcess.ExitCode;
		
		CommandProcess.Close();

		return ExitCode;
	}

	/** Initializes the application by processing command-line arguments, setting up the necessary paths, and initializing logger. */
	private void Initialize(string[] InArguments, Assembly InTargetAssembly)
	{
		CoreAssembly = Assembly.GetExecutingAssembly();
		Verify(CoreAssembly != null, "FlareCore assembly is null!");
		
		TargetAssembly = InTargetAssembly;
		Verify(TargetAssembly != null, "Target assembly is null!");
		
		InitializePaths();
		
		CLog.Initialize(LogsPath, InTargetAssembly.GetName().Name);
	}
	
	/** Shutdown the application. */
	private void Shutdown()
	{
		CLog.Shutdown();
	}
	
	private void InitializePaths()
	{
		SolutionPath = GetSolutionPath();
		
		BinariesPath = Path.Combine(SolutionPath, "Binaries");
		IntermediatePath = Path.Combine(SolutionPath, "Intermediate");
		
		SourcePath = Path.Combine(SolutionPath, "Source");
		BuildRulesPath = Path.Combine(SourcePath, "BuildRules");
		
		SavedPath = Path.Combine(SolutionPath, "Saved");
		LogsPath = Path.Combine(SavedPath, "Logs");
	}
	
	/** Gets solution root directory. */
	private static string GetSolutionPath()
	{
		string CurrentDirectory = Directory.GetCurrentDirectory();
		if (CurrentDirectory.EndsWith("Binaries"))
		{
			return CPath.GetParent(CurrentDirectory);
		}

		if (!CurrentDirectory.EndsWith("FlareHorz"))
		{
			return CPath.GetParent(CurrentDirectory, 3);
		}

		return CurrentDirectory;
	}
}
