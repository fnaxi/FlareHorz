// CopyRight FlareHorz Team. All Rights Reserved.

using System.Diagnostics;
using System.IO;
using FlareCore;
using FlareCore.Logger;

namespace FlareBuildTool.Premake;

/// <summary>
/// Generate solution files with Premake5.
/// </summary>
[FlareBuildToolAPI]
public class FPremakeBuilder : FFlareObject
{
	/// <summary>
	/// Calls when object of that class is created.
	/// </summary>
	protected override void OnObjectCreated()
	{
		base.OnObjectCreated();
	}

	/// <summary>
	/// Generate solution files via Premake5.
	/// <param name="PathToLuaFile"></param>
	/// <param name="VSVersion"></param>
	/// </summary>
	public void GenerateSolutionFiles(string PathToLuaFile, string VSVersion)
	{
		FAssert.Checkf(File.Exists(PathToLuaFile), "Lua file does not exist!");
		FAssert.Check(VSVersion != "");
		
		ExecuteCommand(FGlobal.SolutionPath + @"\FlareBuildTool\ThirdParty\Premake\premake5.exe " + VSVersion + " --file=" + PathToLuaFile);
	}
	
	/// <summary>
	/// Execute a command or executable.
	/// </summary>
	/// <param name="InCommand">Command to execute</param>
	public void ExecuteCommand(string InCommand, ELogVerbosity InLogVerbosity = ELogVerbosity.Info, ELogVerbosity ActionLogVerbosity = ELogVerbosity.NoLogging)
	{
		FAssert.Check(InCommand != "");
		FLog.Log(InLogVerbosity, "Running command: \"" + InCommand + "\"");
		
		// Set timer
		FActionTime AT_CommandExecution = FActionTime.Start("AT_CommandExecution", ActionLogVerbosity);

		// Get process info
		ProcessStartInfo CommandProcessInfo = new ProcessStartInfo("cmd.exe", "/c" + InCommand);

		CommandProcessInfo.CreateNoWindow = true;
		CommandProcessInfo.UseShellExecute = false;
		CommandProcessInfo.RedirectStandardError = true;
		CommandProcessInfo.RedirectStandardOutput = true;

		// Launch process
		Process CommandProcess = Process.Start(CommandProcessInfo);
		FAssert.Checkf(CommandProcess != null, "Process is null!");
		
		// Get output
		CommandProcess.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
		{
			if (!string.IsNullOrEmpty(e.Data))
			{
				FLog.Log(InLogVerbosity, "Output: " + e.Data);
			}
		};
		CommandProcess.BeginOutputReadLine();

		// Get errors
		CommandProcess.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => 
		{
			if (!string.IsNullOrEmpty(e.Data))
			{
				FLog.Error("Error: " + e.Data);
			}
		};
		CommandProcess.BeginErrorReadLine(); // TODO: Does it work in Pre-Build commands?

		// Exit
		CommandProcess.WaitForExit();

		FLog.Log(InLogVerbosity, "Command exited with code " + CommandProcess.ExitCode);
		CommandProcess.Close();
		
		// Stop timer
		AT_CommandExecution.Stop();
	}
}
