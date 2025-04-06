// CopyRight FlareHorz Team. All Rights Reserved.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlareBuildTool.Premake;
using FlareBuildTool.Target;
using FlareBuildTool.Module;
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
		BuildLuaFile = CreateObject<FFile>("BuildLuaFile");
		PremakeAPI = CreateObject<FPremakeAPI>("PremakeAPI");
	}

	/// <summary>
	/// Generate lua file for this solution.
	/// </summary>
	public void GenerateLuaFiles()
	{
		StartGeneratingCode(LuaFile, false);
		StartGeneratingCode(BuildLuaFile, true);
	}

	/// <summary>
	/// Start generating code.
	/// </summary>
	private void StartGeneratingCode(FFile SolutionLuaFile, bool bIsBuildSolution)
	{
		FLog.Info("Generating " + SolutionLuaFile.FileName + " file");
		
		// Open lua file
		PremakeAPI.PremakeLuaFile = SolutionLuaFile;
		PremakeAPI.PremakeLuaFile.LoadFile(PremakeAPI.PremakeLuaFile.FilePath);
		PremakeAPI.PremakeLuaFile.ClearFile();
		
		// Write to that file
		GenerateCode(FGlobal.BuildTool.SolutionTargets, bIsBuildSolution);

		// Close it
		PremakeAPI.PremakeLuaFile.Close();
		
		FLog.Info("Generated " + SolutionLuaFile.FileName);
	}

	/// <summary>
	/// Generate code for lua file.
	/// </summary>
	private void GenerateCode(List<FTarget> Targets, bool bIsBuildSolution)
	{
		PremakeAPI.CopyRight();
		PremakeAPI.Include("FlareBuildTool/FlarePremakeExtension.lua");
		
		GenerateGlobalCode(Targets);

		List<FTarget> BRs = FGlobal.BuildTool.PrecompiledTargets.Where(t => t.Name == "BuildRules").ToList();
		FAssert.Checkf(BRs.Count == 1, "BuildRules target was not created!");
		GenerateBuildRulesCode(BRs[0]);
		
		List<FTarget> CppTargets = Targets.FindAll(t => t.TargetRules.TargetLanguage == ETargetLanguage.CPP);
		foreach (FTarget Target in CppTargets)
		{
			GenerateCodeForCppTarget(Target, bIsBuildSolution);
		}

		List<FTarget> OtherTargets = Targets.FindAll(t => t.TargetRules.TargetLanguage != ETargetLanguage.CPP);
		foreach (FTarget Target in OtherTargets)
		{
			GenerateCodeForTarget(Target);
		}
		
		// TODO: PrecompiledTargets + should ignore BuildRules
	}

	/// <summary>
	/// Generate global code for solution.
	/// </summary>
	private void GenerateGlobalCode(List<FTarget> Targets)
	{
		PremakeAPI.Workspace("FlareHorz");
		
		FTargetRules.SolutionItems.AddRange(new []{ "README.md", ".gitignore" });
		PremakeAPI.SolutionItems(FTargetRules.SolutionItems);

		if (FindStartupTarget(Targets, out var StartupTarget))
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
	}

	/// <summary>
	/// Generate BuildRules solution folder for .Target.cs files of C# targets in the solution.
	/// </summary>
	private void GenerateBuildRulesCode(FTarget BR)
	{
		FTargetRules Rules = BR.TargetRules;
		
		PremakeAPI.Comment("BUILD RULES BEGIN");
		PremakeAPI.GroupStart(Rules.Group);
		{
			PremakeAPI.Project(BR.Name);
			PremakeAPI.Location("Intermediate/" + BR.Name);
			
			PremakeAPI.TargetType(Rules.TargetType);
			PremakeAPI.Language(Rules.TargetLanguage);
			PremakeAPI.CS_Version(Rules.CS_Version);
			PremakeAPI.DotNetFrameworkVersion(Rules.DotNetFrameworkVersion);
			
			PremakeAPI.BinariesPath(Rules.BinariesPath);
			PremakeAPI.IntermediatePath(Rules.IntermediatePath);
			
			PremakeAPI.Files(Rules.Files);
			
			PremakeAPI.Links(Rules.Links);
			
			GenerateConfigurationInfoCode();
			
			PremakeAPI.SystemVersion();
		}
		PremakeAPI.GroupEnd();
		PremakeAPI.Comment("BUILD RULES END");
	}

	/// <summary>
	/// Generate code for one target.
	/// </summary>
	private void GenerateCodeForCppTarget(FTarget Target, bool bIsBuildSolution)
	{
		FTargetRules Rules = Target.TargetRules;
		FAssert.Check(Rules.TargetLanguage == ETargetLanguage.CPP);
		if (bIsBuildSolution)
		{
			GenerateModulesCode(Target);
			
			PremakeAPI.Comment(Target.Name + " C++ TARGET BEGIN");
			PremakeAPI.GroupStart(Rules.Group);

			PremakeAPI.Project(Target.Name);
			PremakeAPI.Location(Target.Name);

			PremakeAPI.TargetType(Rules.TargetType);
			PremakeAPI.Language(ETargetLanguage.CPP);

			PremakeAPI.CPP_Version(Rules.CPP_Version);

			PremakeAPI.BinariesPath(Rules.BinariesPath);
			PremakeAPI.IntermediatePath(Rules.IntermediatePath);

			List<string> Files = Target.TargetRules.Files;
			Files.AddRange(new string[]
			{
				"/" + Target.Name + "/Source/" + Target.Name + ".Target.cs"
			});
			PremakeAPI.Files(Files);

			List<string> Links = Rules.Links;
			Links.AddRange(new []
			{
				"FlareCore",
				"FlareBuildTool",
				"BuildRules"
			});
			PremakeAPI.Links(Links);

			// TODO: LinkedTargets for C++
		}
		else
		{
			PremakeAPI.Comment(Target.Name + " C++ TARGET BEGIN");
			PremakeAPI.GroupStart(Rules.Group);

			PremakeAPI.Project(Target.Name);
			PremakeAPI.Location(Target.Name);

			PremakeAPI.TargetType(Rules.TargetType);
			PremakeAPI.Language(ETargetLanguage.CPP);

			PremakeAPI.CPP_Version(Rules.CPP_Version);

			PremakeAPI.BinariesPath(Rules.BinariesPath);
			PremakeAPI.IntermediatePath(Rules.IntermediatePath);

			List<string> Files = Target.TargetRules.Files;
			Files.AddRange(new string[]
			{
				"/" + Target.Name + "/Source/" + Target.Name + ".Target.cs"
			});
			foreach (FModule Module in Target.Modules)
			{
				Files.AddRange(new string[]
				{
					"/" + Target.Name + "/Source/" + Module.Name + "/" + Module.Name + ".Build.cs",
					"/" + Target.Name + "/Source/" + Module.Name + "/Public/**.h",
					"/" + Target.Name + "/Source/" + Module.Name + "/Private/**.h",
					"/" + Target.Name + "/Source/" + Module.Name + "/Private/**.cpp"
				});
			}

			PremakeAPI.Files(Files);

			PremakeAPI.Links(Rules.Links);

			// TODO: LinkedTargets for C++
		}
		
		GenerateConfigurationInfoCode();
			
		PremakeAPI.SystemVersion();
		
		PremakeAPI.GroupEnd();
		PremakeAPI.Comment(Target.Name + " C++ TARGET END");
	}

	/// <summary>
	/// Generate code for one target.
	/// </summary>
	private void GenerateCodeForTarget(FTarget Target)
	{
		FTargetRules Rules = Target.TargetRules;

		PremakeAPI.Comment(Target.Name + " TARGET BEGIN");
		PremakeAPI.GroupStart(Rules.Group);
		{
			PremakeAPI.Project(Target.Name);
			PremakeAPI.Location(Target.Name);
			
			PremakeAPI.TargetType(Rules.TargetType);
			PremakeAPI.Language(Rules.TargetLanguage);
			switch (Rules.TargetLanguage)
			{
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
			
			GenerateConfigurationInfoCode();
			
			PremakeAPI.SystemVersion();
		}
		PremakeAPI.GroupEnd();
		PremakeAPI.Comment(Target.Name + " TARGET END");
	}

	/// <summary>
	/// Generate code for configuration info.
	/// </summary>
	private void GenerateConfigurationInfoCode()
	{
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
	}

	/// <summary>
	/// Generate code for all modules target have.
	/// </summary>
	private void GenerateModulesCode(FTarget Target)
	{
		foreach (FModule Module in Target.Modules)
		{
			string ModuleNameWrapped = Target.Name + "-" + Module.Name;
			PremakeAPI.Comment(ModuleNameWrapped + " MODULE BEGIN");
			
			PremakeAPI.Comment(ModuleNameWrapped + " MODULE END");
		}
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
	private FTarget FindTargetWithName(string InName, List<FTarget> Targets)
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
	/// Lua file of actual solution we going to build.
	/// This is needed for C++ modules to work.
	/// </summary>
	public FFile BuildLuaFile;

	/// <summary>
	/// Premake5 API.
	/// </summary>
	private FPremakeAPI PremakeAPI;
}
