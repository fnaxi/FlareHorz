// CopyRight FlareHorz Team. All Rights Reserved.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FlareCore;

namespace FlareBuildTool;

public enum EModuleType
{
	Invalid = 0,
	Runtime,
	Editor,
	ThirdParty,
	Game
}
public class CModuleRules
{
	public List<string> PublicDependencyModules = new();
    public List<string> PrivateDependencyModules = new();
   	public List<string> GetDependencyModules() { return Concat(PublicDependencyModules, PrivateDependencyModules); }
    	
    public List<string> PublicDependencyLibraries = new();
    public List<string> PrivateDependencyLibraries = new();
    public List<string> GetDependencyLibraries() { return Concat(PublicDependencyLibraries, PrivateDependencyLibraries); }
	
	public List<string> PublicIncludeDirectories = new();
	public List<string> PrivateIncludeDirectories = new();
	public List<string> GetIncludeDirectories() { return Concat(PublicIncludeDirectories, PrivateIncludeDirectories); }
	
	public List<string> PublicLibraryDirectories = new();
	public List<string> PrivateLibraryDirectories = new();
	public List<string> GetLibraryDirectories() { return Concat(PublicLibraryDirectories, PrivateLibraryDirectories); }
	
	public List<string> PublicDefines = new();
	public List<string> PrivateDefines = new();
	public List<string> GetDefines() { return Concat(PublicDefines, PrivateDefines); }
	
	/** Specifies files to include/exclude in the project. */
	public List<string> FileDirectories = new();
	public List<string> ExcludeFileDirectories = new();
	
	/** The commands to execute on Pre-/Post-build events. */
	public List<string> PreBuildCommands = new();
	public List<string> PostBuildCommands = new();

	/** Specifies that this module contains an entry point. Only one module can set this to true. */
	public bool bStartup = false;
}

public class CThirdPartyModuleRules : CModuleRules
{
	public EBuildOutputType OutputType = EBuildOutputType.None;
}

public class CModule : CBuildItem
{
	public CModule(string InName, string InScriptFilePath, EModuleType InModuleType) : base(InName, InScriptFilePath)
	{
		ModuleType = InModuleType;
		
		CLog.Info($"Instantiated {Name} module of type {ModuleType} with script {CPath.ToFlare(ScriptFilePath)}");

		Rules = GatherRules<CModuleRules>($"BuildRules.{ModuleType}.C{Name}");
		Verify(!(ModuleType == EModuleType.ThirdParty && Rules is not CThirdPartyModuleRules), "Third-party modules must use CThirdPartyModuleRules");
		Verify(!(ModuleType != EModuleType.ThirdParty && Rules is CThirdPartyModuleRules), "Non-third-party modules must use CModuleRules");

		DefaultSetupRules();
	}
	
	public readonly EModuleType ModuleType;
	public readonly CModuleRules Rules;

	private List<string> Links = new();
	
	private const string DllImport = "__declspec(dllimport)";
	private const string DllExport = "__declspec(dllexport)";
	
	private const string ExecutableName = "FlareHorz";
	
	public override string GetRootPath()
	{
		return CPath.FlareCombine(SourcePath, ModuleType.ToString(), Name);
	}

	public override void GeneratePremakeCode(CPremakeFileHandle Premake)
	{
		List<string> Defines = Rules.GetDefines();
		
		List<string> IncludeDirectories = Rules.GetIncludeDirectories();
		List<string> LibraryDirectories = Rules.GetLibraryDirectories();

		EBuildOutputType OutputType;
		if (Rules is CThirdPartyModuleRules ThirdPartyRules)
		{
			Verify(ThirdPartyRules.OutputType != EBuildOutputType.Executable, "Third-party modules can't be an executable!");
			OutputType = ThirdPartyRules.OutputType;
		}
		else
		{
			OutputType = Rules.bStartup ? EBuildOutputType.Executable : EBuildOutputType.DynamicLibrary;
		}
		
		Premake.WriteGroupBegin(ModuleType.ToString());
		CProjectInfo Info = new CProjectInfo
		{
			Name = Name,
			OutputName = Rules.bStartup ? ExecutableName : Name,
			Location = GetRootPath(),
			BinariesPath = CPath.FlareCombine(BinariesPath, "%{cfg.buildcfg}"),
			IntermediatePath = CPath.ToFlare(IntermediatePath),
			OutputType = OutputType,
			Language = ELanguage.Cxx,
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
		
		CLog.Info($"Generated premake code for {Name} module");
	}
	
	public sealed override void DefaultSetupRules()
	{
		if (ModuleType == EModuleType.ThirdParty) return;
		
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
		
		if (!Rules.bStartup)
		{
			// NOTE: In other modules this macro is defined as DllImport
			Rules.PrivateDefines.Add($"{Name.ToUpper()}_API={DllExport}");
		}

		Rules.PrivateDefines.AddRange(new[]
		{
			// Used in IMPLEMENT_MODULE() macro to check that passed module name is correct
			$"FH_MODULE_NAME={Name}",
			"NO_API=",
			
			// TODO: Define WITH_EDITOR only for Editor configurations
			"WITH_EDITOR=1",
			
			// TODO: Cross-platform
			"PLATFORM_WINDOWS=1",
			"FH_PLATFORM_NAME=Windows"
		});
	}
	
	// TODO: Add dependent modules by condition
	public override void SetupRules(in Dictionary<string, CBuildItem> Others)
	{
		List<CModule> Modules = Others.Values.OfType<CModule>().ToList();
		Dictionary<string, CModule> ModuleMap = Modules.ToDictionary(m => m.Name, m => m);
		
		foreach (CModule OtherModule in Modules.Where(m => m != this && !m.Rules.bStartup && !m.IsThirdPartyWithNoOutput() && !m.IsThirdPartyStaticLib()))
		{
			// TODO: Add API macro only for modules that are linked
			
			Rules.PrivateDefines.Add($"{OtherModule.Name.ToUpper()}_API={DllImport}");
		}
		
		// TODO: Revisit code below
		
		// Public dependency modules
		foreach (string OtherModuleName in Rules.PublicDependencyModules)
		{
			CModule OtherModule = FindModuleByName(OtherModuleName, Modules);
			Verify(!OtherModule.Rules.bStartup, "Can't have startup module as a dependency!");
			
			// Include directories
			foreach (string IncludeDirectory in OtherModule.Rules.PublicIncludeDirectories
				         .Where(IncludeDirectory => !Rules.PublicIncludeDirectories.Contains(IncludeDirectory)))
			{
				if (Rules.PrivateIncludeDirectories.Contains(IncludeDirectory))
				{
					Rules.PrivateIncludeDirectories.Remove(IncludeDirectory);
				}
				Rules.PublicIncludeDirectories.Add(IncludeDirectory);
			}
			
			// Library directories
			foreach (string LibraryDirectory in OtherModule.Rules.PublicLibraryDirectories
				         .Where(LibraryDirectory => !Rules.PublicLibraryDirectories.Contains(LibraryDirectory)))
			{
				if (Rules.PrivateLibraryDirectories.Contains(LibraryDirectory))
				{
					Rules.PrivateLibraryDirectories.Remove(LibraryDirectory);
				}
				Rules.PublicLibraryDirectories.Add(LibraryDirectory);
			}

			// Libraries
			foreach (string LibraryName in OtherModule.Rules.PublicDependencyLibraries)
			{
				if (Rules.PrivateDependencyLibraries.Contains(LibraryName))
				{
					Rules.PrivateDependencyLibraries.Remove(LibraryName);
				}
				Rules.PublicDependencyLibraries.Add(LibraryName);
			}
		}
		
		// Private dependency modules
		foreach (string OtherModuleName in Rules.PrivateDependencyModules)
		{
			CModule OtherModule = FindModuleByName(OtherModuleName, Modules);
			Verify(!OtherModule.Rules.bStartup, "Can't have startup module as a dependency!");
			
			// Include directories
			foreach (string IncludeDirectory in OtherModule.Rules.PublicIncludeDirectories
				         .Where(IncludeDirectory => !Rules.GetIncludeDirectories().Contains(IncludeDirectory)))
			{
				Rules.PrivateIncludeDirectories.Add(IncludeDirectory);
			}
			
			// Library directories
			foreach (string LibraryDirectory in OtherModule.Rules.PublicLibraryDirectories
				         .Where(LibraryDirectory => !Rules.GetLibraryDirectories().Contains(LibraryDirectory)))
			{
				Rules.PrivateLibraryDirectories.Add(LibraryDirectory);
			}
			
			// Libraries
			foreach (string LibraryName in OtherModule.Rules.PublicDependencyLibraries
				         .Where(LibraryName => !Rules.GetDependencyLibraries().Contains(LibraryName)))
			{
				Rules.PrivateDependencyLibraries.Add(LibraryName);
			}
		}
		
		// TODO: Revisit this. Dependencies are only half implemented
		Links.AddRange( GatherDependencies_BFS(ModuleMap) );
		
		CLog.Info($"SetupRules() completed for {Name} module");
	}
	
	/** Gather dependencies for this module using BFS (Breadth-First Search) algorithm. */
	private List<string> GatherDependencies_BFS(Dictionary<string, CModule> AllModules)
	{
		List<string> Dependencies = new();
		HashSet<string> Visited = new();
		
		Queue<CModule> Queue = new();
		Queue.Enqueue(this);

		while (Queue.Count > 0)
		{
			CModule CurrentModule = Queue.Dequeue();

			foreach (string DepName in CurrentModule.Rules.GetDependencyModules())
			{
				if (Visited.Contains(DepName))
					continue;

				if (!AllModules.TryGetValue(DepName, out CModule DepModule))
					continue;

				Visited.Add(DepName);

				if (!DepModule.IsThirdPartyWithNoOutput())
				{
					Dependencies.Add(DepModule.Name);
					CLog.Verbose($"{Name} module: Added {DepName} module dependency");
				}
				else
				{
					Dependencies.AddRange(DepModule.Rules.PublicDependencyLibraries);
					foreach (string LibName in DepModule.Rules.PublicDependencyLibraries)
					{
						CLog.Verbose($"{Name} module: Added {LibName} library dependency");
					}
				}

				Queue.Enqueue(DepModule);
			}
		}
		return Dependencies;
	}

	private static CModule FindModuleByName(string InName, in List<CModule> Modules)
	{
		CModule Module = Modules.Find(m => m.Name == InName);
		
		CLog.Verbose($"FindModuleByName({InName}) was called");
		Verify(Module != null, $"Could not find module with name: {InName}");

		return Module;
	}
	
	private bool IsThirdPartyWithNoOutput()
	{
		return ModuleType == EModuleType.ThirdParty && Rules is CThirdPartyModuleRules { OutputType: EBuildOutputType.None };
	}
	
	private bool IsThirdPartyStaticLib()
	{
		return ModuleType == EModuleType.ThirdParty && Rules is CThirdPartyModuleRules { OutputType: EBuildOutputType.StaticLibrary };
	}
}
