// CopyRight FlareHorz Team. All Rights Reserved.

using System.Collections.Generic;
using FlareBuildTool.Premake;
using FlareBuildTool.Target;
using FlareCore;
using FlareCore.Logger;

namespace FlareBuildTool.Solution;

/// <summary>
/// A representation of solution we need to parse and build.
/// Contains info about all targets.
/// </summary>
[FlareBuildToolAPI]
public class FSolution : FFlareObject
{
	/// <summary>
	/// Calls when object of that class is created.
	/// </summary>
	protected override void OnObjectCreated()
	{
		base.OnObjectCreated();
		
		LuaFile = CreateObject<FFile>("LuaFile");
		PremakeAPI = CreateObject<FPremakeAPI>("PremakeAPI");
	}

	/// <summary>
	/// Generate lua file for this solution.
	/// </summary>
	public void GenerateLuaFile()
	{
		FLog.Info("Generating " + LuaFile.FileName + " file");
		
		// Open lua file
		PremakeAPI.PremakeLuaFile = LuaFile;
		PremakeAPI.PremakeLuaFile.LoadFile(PremakeAPI.PremakeLuaFile.FilePath);
		PremakeAPI.PremakeLuaFile.ClearFile();
		
		// Write to that file
		GenerateCode(FGlobal.BuildTool.SolutionTargets);

		// Close it
		PremakeAPI.PremakeLuaFile.Close();
		
		FLog.Info("Generated " + LuaFile.FileName);
	}

	/// <summary>
	/// Generate code for lua file.
	/// </summary>
	private void GenerateCode(List<FTarget> Targets)
	{
		foreach (FTarget Target in Targets)
		{
			Target.PrintTargetInfo();
		}
		
		PremakeAPI.CopyRight();
		PremakeAPI.Include("FlareBuildTool/FlarePremakeExtension.lua");
		
		GenerateGlobalCode(Targets);
	}

	/// <summary>
	/// Generate global code for solution.
	/// </summary>
	private void GenerateGlobalCode(List<FTarget> Targets)
	{
		PremakeAPI.Workspace("FlareHorz");
		PremakeAPI.SolutionItems(new string[]
		{
			"README.md", ".gitignore"
		});
		
		GenerateBuildRulesCode(Targets);

		FTarget StartupTarget;
		if (FindStartupTarget(Targets, out StartupTarget))
		{
			PremakeAPI.StartupProject(StartupTarget.Name);
		}
		
		PremakeAPI.Configurations(new string[]
		{
			"Debug",
			"Development"
		});
		PremakeAPI.Platforms(new string[]
		{
			"Win64"
		});
		
		PremakeAPI.StrVariable("v_OutputDir", "%{cfg.buildcfg}");

		foreach (FTarget Target in Targets)
		{
			GenerateCodeForTarget(Target);
		}
	}

	/// <summary>
	/// Generate BuildRules solution folder for .Target.cs files of C# targets in the solution.
	/// </summary>
	private void GenerateBuildRulesCode(List<FTarget> Targets)
	{
		List<FTarget> CSTargets = Targets.FindAll(t => t.TargetRules.TargetLanguage == ETargetLanguage.CS);
		List<string> TargetCsFiles = new List<string>();
		foreach (FTarget Target in CSTargets)
		{
			string PathToFile = Target.Name + "/"  + Target.Name + ".Target.cs";
			TargetCsFiles.Add(PathToFile);
		}

		PremakeAPI.BuildRules(TargetCsFiles);
	}

	/// <summary>
	/// Generate code for one target.
	/// </summary>
	private void GenerateCodeForTarget(FTarget Target)
	{
		FTargetRules Rules = Target.TargetRules;
		
		PremakeAPI.Comment("TARGET BEGIN");
		PremakeAPI.GroupStart(Rules.Group);
		{
			PremakeAPI.Project(Target.Name);
			PremakeAPI.Location(Target.Name);
			
			PremakeAPI.TargetType(Rules.TargetType);
			PremakeAPI.Language(Rules.TargetLanguage);
			switch (Rules.TargetLanguage)
			{
				case ETargetLanguage.CPP:
					PremakeAPI.CPP_Version(Rules.CPP_Version);
					break;
				case ETargetLanguage.CS:
					PremakeAPI.CS_Version(Rules.CS_Version);
					PremakeAPI.DotNetFrameworkVersion(Rules.DotNetFrameworkVersion);
					break;
				default:
					FAssert.CheckNoEntry();
					break;
			}
			
			PremakeAPI.BinariesPath(Rules.BinariesPath);
			PremakeAPI.IntermediatePath(Rules.IntermediatePath);
			
			PremakeAPI.Files(ExtendTargetFiles(Target));

			if (Rules.TargetLanguage == ETargetLanguage.CS)
			{
				PremakeAPI.Exlude("/" + Target.Name + "/" + Target.Name + ".Target.cs");
			}
			
			PremakeAPI.Links(Rules.Links);
			
			// TODO: LinkedTargets for C++
			
			// DEBUG
			PremakeAPI.FilterConfiguration(EConfiguration.Debug);
			{
				PremakeAPI.Defines(new string[]
				{
					GetConfigurationDefine(EConfiguration.Debug)
				});
				PremakeAPI.Runtime(EConfiguration.Debug);
				PremakeAPI.COND_Symbols();
			}
			// DEVELOPMENT
			PremakeAPI.FilterConfiguration(EConfiguration.Development);
			{
				PremakeAPI.Defines(new string[]
				{
					GetConfigurationDefine(EConfiguration.Development)
				});
				PremakeAPI.Runtime(EConfiguration.Shipping);
				PremakeAPI.COND_Optimize();
			}
			// SHIPPING
			PremakeAPI.FilterConfiguration(EConfiguration.Shipping);
			{
				PremakeAPI.Defines(new string[]
				{
					GetConfigurationDefine(EConfiguration.Shipping)
				});
				PremakeAPI.Runtime(EConfiguration.Shipping);
				PremakeAPI.COND_Optimize();
			}
			
			PremakeAPI.SystemVersion();
		}
		PremakeAPI.GroupEnd();
		PremakeAPI.Comment("TARGET END");
	}

	/// <summary>
	/// Get a define for configuration.
	/// </summary>
	/// <returns>A "FH_{define}" define with selected configuration.</returns>
	private string GetConfigurationDefine(EConfiguration Configuration)
	{
		return "FH_" + Configuration.ToString().ToUpper();
	}

	/// <summary>
	/// Extend files field of target with default ones.
	/// </summary>
	/// <param name="Target">For which target.</param>
	/// <returns>Files field of that target</returns>
	public List<string> ExtendTargetFiles(FTarget Target)
	{
		List<string> Files = Target.TargetRules.Files;
		switch (Target.TargetRules.TargetLanguage)
		{
			case ETargetLanguage.CPP:
				// TODO: new directory for each module
				break;
			case ETargetLanguage.CS:
				Files.AddRange(new string[]
				{
					"/" + Target.Name + "/**.cs"
				});
				break;
			default:
				FAssert.CheckNoEntry();
				break;
		}
		
		return Files;
	}

	/// <summary>
	/// Find target which have bStartupTarget set on.
	/// <param name="OutStartupTarget">Startup target we found.</param>
	/// <returns>True if startup target found succsesfully.</returns>
	/// </summary>
	private bool FindStartupTarget(List<FTarget> Targets, out FTarget OutStartupTarget)
	{
		List<FTarget> StartupTargets = Targets.FindAll(t => t.TargetRules.bStartupTarget);
		if (FAssert.Verifyf(StartupTargets.Count >= 1, "No startup target found. One target should have bStartupProject set!"))
		{
			FAssert.Checkf(StartupTargets.Count == 1, "Multiple startup targets found. There should be only one startup target!");

			OutStartupTarget = StartupTargets[0];
			return true;
		}
		OutStartupTarget = null;
		return false;
	}
	
	/// <summary>
	/// Search for target using it's name. Should be only one target with this name.
	/// </summary>
	/// <param name="InName">Name to search with.</param>
	/// <returns>Target which have this name.</returns>
	private FTarget SearchForTargetUsingName(string InName, List<FTarget> Targets)
	{
		List<FTarget> FoundTargets = Targets.FindAll(t => t.Name == InName);
		
		// Throw assert if failed and if there is two targets with same name
		FAssert.Checkf(FoundTargets.Count != 0, "There are no targets found with " + InName + " name!");
		FAssert.Checkf(FoundTargets.Count == 1, "Found more than one target with " + InName + " name!");
		
		return FoundTargets[0];
	}
	
	/// <summary>
	/// Lua file of this solution.
	/// </summary>
	public FFile LuaFile;

	/// <summary>
	/// Premake5 API.
	/// </summary>
	private FPremakeAPI PremakeAPI;
}
