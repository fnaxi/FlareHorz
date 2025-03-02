// CopyRight FlareHorz Engine Development Team. All Rights Reserved.

using FlareCore;
using FlareCore.Logger;

namespace FlareBuildTool.EntryPoint;

/// <summary>
/// The entry point of the application.
/// </summary>
[FlareBuildToolAPI]
static class FProgram
{
	/// <summary>
	/// Build tool.
	/// </summary>
	static private FBuildTool BuildTool = new FBuildTool();
	
	/// <summary>
	/// Automatically invoked when the application is started.
	/// </summary>
	/// <param name="Args">An array of command-line arguments passed to the application</param>
	static Int32 Main(string[] Args)
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
		Int32 ExitResult = BuildTool.GuardedMain(Args.Length, Args);
		
		// Shutdown logger
		FLog.Shutdown();
		
		return ExitResult;
	}
	
	/// <summary>
	/// Gets solution directory. We dedicated that code to dedicated method because if you run
	/// FlareBuildTool from IDE, cmd or by .exe solution directory will be different always.
	/// </summary>
	private static string GetSolutionDirectory()
	{
		return FFile.GetParent(Directory.GetCurrentDirectory(), 4);
	}
}
