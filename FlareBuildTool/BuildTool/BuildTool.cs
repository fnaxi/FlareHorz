// CopyRight FlareHorz Team. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlareBuildTool.Premake;
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
///		Target/
///			Module/
///				.Build.cs
///			.Target.cs
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
		PremakeBuilder = CreateObject<FPremakeBuilder>("PremakeBuilder");
	}
	
	/// <summary>
	/// Describes solution we build.
	/// </summary>
	public FSolution Solution;
	
	/// <summary>
	/// Targets this solution have.
	/// </summary>
	public List<FTarget> SolutionTargets;

	/// <summary>
	/// Premake5 builder.
	/// </summary>
	private FPremakeBuilder PremakeBuilder;
	
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
			// There should be at least FlareBuildTool and FlareCore
			FLog.Warn("No targets found! Exiting...");
			return 1;
		}
		else
		{
			FLog.Info("Found " + TargetPaths.Length + " targets");
		}

		// Collect info about each target and save into FTarget
		foreach (string TargetPath in TargetPaths)
		{
			// Find name of that target
			string TargetName = Path.GetFileNameWithoutExtension(TargetPath);
			
			// Create a FTarget instance to store data
			FTarget Target = CreateObject<FTarget>(TargetName + "Target");
			Target.Name = TargetName;
			
			// Save info about .Target.cs location
			Target.TargetCsFile.LoadFile(Path.Combine(TargetPath, TargetName + ".Target.cs"));
			Target.TargetCsFile.Close();
			
			// Compile .Target.cs file at runtime
			Target.CompileBuildFile();
			
			// Handle modules in that target
			Target.HandleModules();
			
			// Update solution targets
			SolutionTargets.Add(Target);
		}
		
		// Generate FlareHorz.sln.lua
		string SolutionLuaFileName = "FlareHorz.sln.lua";
		string SolutionLuaFilePath = Path.Combine(FGlobal.SolutionPath, SolutionLuaFileName);
		if (File.Exists(SolutionLuaFilePath))
		{
			Solution.LuaFile.LoadFile(SolutionLuaFilePath);
		}
		else
		{
			Solution.LuaFile.CreateFile(FGlobal.SolutionPath, SolutionLuaFileName);
		}
		Solution.LuaFile.Close();
		Solution.GenerateLuaFile();
		
		// Generate project files with Premake5
		PremakeBuilder.GenerateSolutionFiles(Solution.LuaFile.FilePath, "vs2022");
		
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
		string[] PossibleTargetPaths = Directory.GetDirectories(FGlobal.SolutionPath)
			.Where(s => !(s.EndsWith(".git")))
			.Where(s => !(s.EndsWith(".github")))
			.Where(s => !(s.EndsWith(".idea")))
			.Where(s => !(s.EndsWith(".vs")))
			.Where(s => !(s.EndsWith(".vscode")))
			.Where(s => !(s.EndsWith("images")))
			.Where(s => !(s.EndsWith("Binaries")))
			.Where(s => !(s.EndsWith("Intermediate")))
			.ToArray();

		// Check is there .Target.cs if yes then it's FlareHorz target
		List<string> CheckedTargetPaths = new List<string>();
		foreach (string TargetPath in PossibleTargetPaths)
		{
			string TargetName = TargetPath.Split('\\').Last();
			
			string TargetCsPath = Path.Combine(TargetPath, TargetName + ".Target.cs");
			if (File.Exists(TargetCsPath))
			{
				CheckedTargetPaths.Add(TargetPath);
			}
			else
			{
				FLog.Warn(TargetName + " is not a target!");
			}
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
