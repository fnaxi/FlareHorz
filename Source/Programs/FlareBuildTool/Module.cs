// CopyRight FlareHorz Team. All Rights Reserved.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FlareCore;

namespace FlareBuildTool;

public enum EModuleType
{
	Runtime,
	Editor,
	ThirdParty,
	Game
}

public class CModuleRules
{
	public List<string> PublicDependencyModules = new();
	public List<string> PrivateDependencyModules = new();
	public List<string> GetDependencyModules()
	{
		return Concat(PublicDependencyModules, PrivateDependencyModules);
	}
	
	public List<string> PublicDependencyLibraries = new();
	public List<string> PrivateDependencyLibraries = new();
	public List<string> GetDependencyLibraries()
	{
		return Concat(PublicDependencyLibraries, PrivateDependencyLibraries);
	}
	
	public List<string> PublicIncludeDirectories = new();
	public List<string> PrivateIncludeDirectories = new();
	public List<string> GetIncludeDirectories()
	{
		return Concat(PublicIncludeDirectories, PrivateIncludeDirectories);
	}
	
	public List<string> PublicLibraryDirectories = new();
	public List<string> PrivateLibraryDirectories = new();
	public List<string> GetLibraryDirectories()
	{
		return Concat(PublicLibraryDirectories, PrivateLibraryDirectories);
	}
	
	public List<string> PublicDefines = new();
	public List<string> PrivateDefines = new();
	public List<string> GetDefines()
	{
		return Concat(PublicDefines, PrivateDefines);
	}
	
	/** Specifies files to include/exclude in the project. */
	public List<string> FileDirectories = new();
	public List<string> ExcludeFileDirectories = new();
	
	/** The commands to execute on Pre-/Post-build events. */
	public List<string> PreBuildCommands = new();
	public List<string> PostBuildCommands = new();

	/** Specifies that this module contains an entry point. Only one module can set this to true. */
	public bool bStartup = false;
}

public class CModule : CBuildItem
{
	public CModule(string InScriptFilePath, EModuleType InModuleType) : base(InScriptFilePath)
	{
		CLog.Info($"Instantiated {Name} module of type {ModuleType} with script {ScriptFilePath}");
		
		ModuleType = InModuleType;
		Rules = GatherRules<CModuleRules>($"BuildRules.{ModuleType}.C{Name}");
		DefaultSetupRules();
	}
	
	public readonly EModuleType ModuleType;
	public readonly CModuleRules Rules;
	
	private const string DllImport = "__declspec(dllimport)";
	private const string DllExport = "__declspec(dllexport)";
	
	private const string ExecutableName = "FlareHorz";
	
	public override string GetRootPath()
	{
		return CPath.FlareCombine(SourcePath, ModuleType.ToString(), Name);
	}

	public override void GeneratePremakeCode(CPremakeFileHandle Premake)
	{
		List<string> Links = Concat(Rules.GetDependencyModules(), Rules.GetLibraryDirectories());
		List<string> Defines = Rules.GetDefines();
		
		List<string> IncludeDirectories = Rules.GetIncludeDirectories();
		List<string> LibraryDirectories = Rules.GetLibraryDirectories();
		
		Premake.WriteGroupBegin(ModuleType.ToString());
		CProjectInfo Info = new CProjectInfo
		{
			Name = Name,
			OutputName = Rules.bStartup ? ExecutableName : Name,
			Location = GetRootPath(),
			BinariesPath = CPath.FlareCombine(BinariesPath, "%{cfg.buildcfg}"),
			IntermediatePath = CPath.FlareCombine(IntermediatePath, "%{cfg.buildcfg}"),
			OutputType = Rules.bStartup ? EBuildOutputType.Executable : EBuildOutputType.DynamicLibrary,
			Language = ETargetLanguage.Cxx,
			FileDirectories = Rules.FileDirectories,
			ExcludeFileDirectories = Rules.ExcludeFileDirectories,
			Links = Links,
			Defines = Defines,
			IncludeDirectories = IncludeDirectories,
			LibraryDirectories = LibraryDirectories,
			PreBuildCommands = Rules.PreBuildCommands,
			PostBuildCommands = Rules.PostBuildCommands,
			CustomCode = () =>
			{
				Premake.WriteRemovePlatforms(new List<string>
				{
					"x64"
				});
			}
		};
		Premake.WriteProjectCode(Info);
		Premake.WriteGroupEnd();
		
		CLog.Verbose($"GeneratePremakeCode() completed for {Name} module");
	}

	public override void SetupRules(in List<CBuildItem> Others)
	{
		List<CModule> Modules = Others.OfType<CModule>().ToList();
		
		foreach (CModule OtherModule in Modules.Where(m => !m.Rules.bStartup))
		{
			Rules.PrivateDefines.Add($"{OtherModule.Name.ToUpper()}_API={DllImport}");
		}
		
		foreach (string OtherModuleName in Rules.PublicDependencyModules)
		{
			CModule OtherModule = GetModuleByName(OtherModuleName, Modules);
			Verify(!OtherModule.Rules.bStartup, "Can't have startup module as a dependency!");
			
			foreach (string IncludeDirectory in OtherModule.Rules.PublicIncludeDirectories
				         .Where(IncludeDirectory => !Rules.PublicIncludeDirectories.Contains(IncludeDirectory)))
			{
				if (Rules.PrivateIncludeDirectories.Contains(IncludeDirectory))
				{
					Rules.PrivateIncludeDirectories.Remove(IncludeDirectory);
				}
				Rules.PublicIncludeDirectories.Add(IncludeDirectory);
			}
		}
		foreach (string OtherModuleName in Rules.PrivateDependencyModules)
		{
			CModule OtherModule = GetModuleByName(OtherModuleName, Modules);
			foreach (string IncludeDirectory in OtherModule.Rules.PublicIncludeDirectories
				         .Where(IncludeDirectory => !Rules.GetIncludeDirectories().Contains(IncludeDirectory)))
			{
				Rules.PrivateIncludeDirectories.Add(IncludeDirectory);
			}
		}
		
		CLog.Verbose($"SetupRules() completed for {Name} module");
	}
	public sealed override void DefaultSetupRules()
	{
		Rules.FileDirectories.AddRange(new[]
		{
			CPath.FlareCombine(GetRootPath(), "Public", "**.h"),
			CPath.FlareCombine(GetRootPath(), "Private", "**.h"),
			CPath.FlareCombine(GetRootPath(), "Private", "**.cpp")
		});
		Rules.PublicIncludeDirectories.AddRange(new[]
		{
			CPath.FlareCombine(GetRootPath(), "Public")
		});
		Rules.PreBuildCommands.AddRange(new[]
		{
			CPath.FlareCombine(BinariesPath, $"{TargetAssembly.GetName().Name}.exe")
		});
		
		// NOTE: In other modules this macro is defined as DllImport
		if (!Rules.bStartup)
		{
			Rules.PrivateDefines.Add($"{Name.ToUpper()}_API={DllExport}");
		}

		Rules.PrivateDefines.AddRange(new[]
		{
			// Used in IMPLEMENT_MODULE() macro to check that passed name is correct
			$"FH_MODULE_NAME={Name}",
			
			// TODO: Cross-platform
			"PLATFORM_WINDOWS=1",
			"FH_PLATFORM_NAME=Windows"
		});
	}
	
	private static CModule GetModuleByName(string InName, in List<CModule> Modules)
	{
		CModule Module = Modules.Find(m => m.Name == InName);
		Verify(Module != null, $"Could not find Module: {InName}");
		
		return Module;
	}
}
