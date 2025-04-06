// CopyRight FlareHorz Team. All Rights Reserved.

using System;
using System.IO;
using System.Linq;
using FlareCore;
using FlareCore.Logger;

namespace FlareBuildTool;

#if FH_SHIPPING
	#warning Building FlareBuildTool in Shipping configuration!
#endif

/// <summary>
/// The entry point of the application.
/// </summary>
[FlareBuildToolAPI]
static class FProgram
{
	/// <summary>
	/// Automatically invoked when the application is started.
	/// </summary>
	/// <param name="Args">An array of command-line arguments passed to the application</param>
	static int Main(string[] Args)
	{
		// Find solution path
		FGlobal.SolutionPath = GetSolutionDirectory();
		
		// Initialize logger
		FLog.Initialize(FGlobal.SolutionPath, "FlareBuildTool");
		FLog.Info("SolutionPath: " + FGlobal.SolutionPath);
		
		// Get and check solution path
		FGlobal.SolutionName = FGlobal.SolutionPath.Split(@"\"[0]).Last();
		FAssert.Checkf(FGlobal.SolutionName == "FlareHorz", "Failed to get solution path!");
		
		// Run build tool
		int ExitResult = FGlobal.BuildTool.GuardedMain(Args.Length, Args);
		FLog.Log((ExitResult == 0 ? ELogVerbosity.Info : ELogVerbosity.Error), "Exited with code " + ExitResult + "!");
		
		// Shutdown logger
		FLog.Shutdown();
		
		return ExitResult;
	}
	
	/// <summary>
	/// Gets solution directory.
	/// </summary>
	private static string GetSolutionDirectory()
	{
		// return FFile.GetParent(Directory.GetCurrentDirectory(), 4);
		return FFile.GetParent(Directory.GetCurrentDirectory(), 2);
	}
}
