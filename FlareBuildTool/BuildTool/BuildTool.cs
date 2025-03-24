// CopyRight FlareHorz Team. All Rights Reserved.

using FlareBuildTool.Solution;
using FlareBuildTool.Target;
using FlareCore;
using FlareCore.Logger;

namespace FlareBuildTool;

/// <summary>
/// Build tool class.
/// Combines all actions needed to be done to build entire solution.
/// Actually the scheme of building generally looks like this:
/// Solution:
///		Target (Project)
///			Module
/// </summary>
[FlareBuildToolAPI]
public class FBuildTool : FFlareObject
{
	/// <summary>
	/// Calls when object of that class is created.
	/// </summary>
	protected override void OnObjectCreated()
	{ 
		base.OnObjectCreated();
		
		SolutionTargets = new List<FTarget>();
		Solution = CreateObject<FSolution>("Solution");
		MinimalSolution = CreateObject<FSolution>("MinimalSolution");
	}
	
	/// <summary>
	/// Describes solution we build.
	/// </summary>
	public FSolution Solution;
	
	/// <summary>
	/// A solution to build Flare Build Tool.
	/// </summary>
	public FSolution MinimalSolution;
	
	/// <summary>
	/// Targets this solution have.
	/// </summary>
	private List<FTarget> SolutionTargets;
	
	/// <summary>
	/// Run the build process.
	/// <param name="ArgumentsCount">Count of arguments.</param>
	/// <param name="Arguments">Console-line arguments.</param>
	/// </summary>
	public Int32 GuardedMain(Int32 ArgumentsCount, string[] Arguments)
	{
		// Search for targets
		string[] TargetPaths = SearchForTargets();
		if (TargetPaths.Length == 0)
		{
			FLog.Warn("No targets found! Exiting...");
			return 1;
		}
		
		return 0;
	}
	
	/// <summary>
	/// Search for all folders in solution to find all targets.
	/// Also check is there .Target.cs in each folder to make sure it's FlareHorz target.
	/// </summary>
	/// <returns>Paths of targets found.</returns>
	private string[] SearchForTargets()
	{
		// Search for all folders that can be targets
		string[] TargetPaths = Directory.GetDirectories(FGlobal.SolutionPath)
			.Where(s => !(s.EndsWith(".git")))
			.Where(s => !(s.EndsWith(".github")))
			.Where(s => !(s.EndsWith(".idea")))
			.Where(s => !(s.EndsWith(".vs")))
			.Where(s => !(s.EndsWith(".vscode")))
			.Where(s => !(s.EndsWith("images")))
			.Where(s => !(s.EndsWith("Binaries")))
			.Where(s => !(s.EndsWith("FlareCore")))       // C# begin
			.Where(s => !(s.EndsWith("FlareBuildTool")))
			.Where(s => !(s.EndsWith("FlareHeaderTool"))) // C# end
			.ToArray();

		// Check is there .Target.cs if yes then it's FlareHorz target
		List<string> CheckedTargetPaths = new List<string>();
		foreach (string TargetPath in TargetPaths)
		{
			string TargetName = TargetPath.Split('\\').Last();
			if (File.Exists(Path.Combine(TargetPath, TargetName + ".Target.cs")))
			{
				CheckedTargetPaths.Add(TargetPath);
			}
		}
		
		// Log target paths
		foreach (string TargetPath in TargetPaths)
		{
			FLog.Debug("Target: " + TargetPath);
		}
		
		// Return it
		return CheckedTargetPaths.ToArray();
	}

	/// <summary>
	/// Search for FlareHorz targets (which have .Target.cs) in ThirdParty/ folder.
	/// </summary>
	private string[] SearchForThirdPartyTargets()
	{
		// TODO: ThirdParty/ targets
		return new[] { "" };
	}
}
