// CopyRight FlareHorz Team. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlareBuildTool.Module;
using FlareBuildTool.Premake;
using FlareBuildTool.Solution;
using FlareBuildTool.Target;
using FlareCore;
using FlareCore.Logger;

namespace FlareBuildTool.BuildTool;

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
		PrecompiledTargets = new List<FTarget>();
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
	/// Precompiled targets this solution have.
	/// For example, BuildRules.
	/// </summary>
	public List<FTarget> PrecompiledTargets;

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
			FLog.Warn("No targets found! Exiting...");
			return 1;
		}
		
		FLog.Info("Found " + TargetPaths.Length + " targets");
		
		// Collect info about each target and save into FTarget
		foreach (string TargetPath in TargetPaths)
		{
			// Update solution targets
			SolutionTargets.Add(InitializeTarget(TargetPath));
		}
		
		// Build Rules
		PrecompiledTargets.Add(InitializeBuildRules());

		// Load solution lua files
		LoadSolutionLuaFile(Solution.LuaFile, "FlareHorz.sln.lua");
		LoadSolutionLuaFile(Solution.BuildLuaFile, "FlareHorz.sln.build.lua");
		
		// Generate lua files
		Solution.GenerateLuaFiles();
		
		// Generate project files with Premake5
		PremakeBuilder.GenerateSolutionFiles(Solution.LuaFile.FilePath, "vs2022");
		
		return 0;
	}

	/// <summary>
	/// Initialize target found in TargetPath.
	/// </summary>
	private FTarget InitializeTarget(string TargetPath)
	{
		// Find name of that target
		string TargetName = Path.GetFileNameWithoutExtension(TargetPath);
			
		// Create a FTarget instance to store data
		FTarget Target = CreateObject<FTarget>(TargetName + "Target");
		Target.Name = TargetName;
			
		// Save info about .Target.cs location
		string TargetCsPath = Path.Combine(TargetPath, TargetName + ".Target.cs");
		string CppTargetCsPath = Path.Combine(TargetPath, "Source", TargetName + ".Target.cs");
		Target.TargetCsFile.LoadFile(File.Exists(TargetCsPath) ? TargetCsPath : CppTargetCsPath);
		Target.TargetCsFile.Close();
			
		// Compile .Target.cs file at runtime
		Target.CompileBuildFile();
			
		// Handle modules in that target
		Target.HandleModules();

		return Target;
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
			string CppTargetCsPath = Path.Combine(TargetPath, "Source", TargetName + ".Target.cs");
			
			if (File.Exists(TargetCsPath) || File.Exists(CppTargetCsPath))
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

	/// <summary>
	/// Initialize build rules.
	/// </summary>
	private FTarget InitializeBuildRules()
	{
		FTarget BuildRulesTarget = CreateObject<FTarget>("BuildRulesTarget");
		BuildRulesTarget.Name = "BuildRules";
		BuildRulesTarget.TargetRules.TargetLanguage = ETargetLanguage.CS;
		BuildRulesTarget.TargetRules.TargetType = ETargetType.DynamicLibrary;
		BuildRulesTarget.TargetRules.Group = "BuildRules";
		BuildRulesTarget.TargetRules.Links.AddRange(new []
		{
			"FlareCore",
			"FlareBuildTool"
		});

		List<string> TargetCsFiles = new List<string>();
		foreach (FTarget Target in SolutionTargets)
		{
			switch (Target.TargetRules.TargetLanguage)
			{
				case ETargetLanguage.CPP:
					TargetCsFiles.Add("/" + Target.Name + "/Source/" + Target.Name + ".Target.cs");
					foreach (FModule Module in Target.Modules)
					{
						TargetCsFiles.Add("/" + Target.Name + "/Source/" + Module.Name + "/" + Module.Name + ".Build.cs");
					}
					break;
				case ETargetLanguage.CS:
					TargetCsFiles.Add("/" + Target.Name + "/" + Target.Name + ".Target.cs");
					break;
				default:
					FAssert.CheckNoEntry();
;					break;
			}
		}
		
		foreach (string TargetCsFile in TargetCsFiles)
		{
			string Path = TargetCsFile;
			BuildRulesTarget.TargetRules.Files.Add(Path);
		}

		return BuildRulesTarget;
	}

	/// <summary>
	/// Load information about solution lua file or create it.
	/// </summary>
	private void LoadSolutionLuaFile(FFile Source, string FileName)
	{
		string FilePath = Path.Combine(FGlobal.SolutionPath, FileName);
		if (File.Exists(FilePath))
		{
			Source.LoadFile(FilePath);
		}
		else
		{
			Source.CreateFile(FGlobal.SolutionPath, FileName);
		}
		Source.Close();
	}
}
