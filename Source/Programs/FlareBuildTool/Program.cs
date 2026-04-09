// CopyRight FlareHorz Team. All Rights Reserved.

using System.Collections.Generic;
using System.Linq;
using FlareCore;

namespace FlareBuildTool;

/**
 * The output type produced by building a project.
 */
public enum EBuildOutputType
{
	/** No output or unknown build output. */
	None,
	
	/** A standalone executable (EXE, ELF, etc.). */
	Executable,

	/** A dynamic/shared library (DLL, .so, .dylib). */
	DynamicLibrary,

	/** A static library (e.g., .lib, .a). */
	StaticLibrary
}

public class CProgramRules
{
	public EBuildOutputType OutputType;
	
	public List<string> Dependencies = new();
	public List<string> Defines = new();
	
	/** Specifies files to include/exclude in the project. */
	public List<string> FileDirectories = new();
	public List<string> ExcludeFileDirectories = new();
	
	/** The commands to execute on Pre-/Post-build events. */
	public List<string> PreBuildCommands = new();
	public List<string> PostBuildCommands = new();
}

public class CProgram : CBuildItem
{
	public CProgram(string InScriptFilePath) : base(InScriptFilePath)
	{
		CLog.Info($"Instantiated {Name} program with script {ScriptFilePath}");

		Rules = GatherRules<CProgramRules>(IsBuildRulesProgram() ? $"BuildRules.C{Name}" : $"BuildRules.Programs.C{Name}");
		DefaultSetupRules();
	}

	public readonly CProgramRules Rules;
	
	public override string GetRootPath()
	{
		return IsBuildRulesProgram()
			? CPath.FlareCombine(SourcePath, Name) 
			: CPath.FlareCombine(SourcePath, "Programs", Name);
	}

	public override void GeneratePremakeCode(CPremakeFileHandle Premake)
	{
		if (!IsBuildRulesProgram())
		{
			Premake.WriteGroupBegin("Programs");
		}

		CProjectInfo Info = new CProjectInfo
		{
			Name = Name,
			OutputName = Name,
			Location = GetRootPath(),
			BinariesPath = CPath.ToFlare(BinariesPath),
			IntermediatePath = CPath.ToFlare(IntermediatePath),
			OutputType = Rules.OutputType,
			Language = ETargetLanguage.CSharp,
			FileDirectories = Rules.FileDirectories,
			ExcludeFileDirectories = Rules.ExcludeFileDirectories,
			Links = Rules.Dependencies,
			Defines = Rules.Defines,
			IncludeDirectories = new List<string>(),
			LibraryDirectories = new List<string>(),
			PreBuildCommands = Rules.PreBuildCommands,
			PostBuildCommands = Rules.PostBuildCommands,
			CustomCode = () =>
			{
				Premake.WriteRemovePlatforms(new List<string>()
				{
					"Win64"
				});
			}
		};
		Premake.WriteProjectCode(Info);

		if (!IsBuildRulesProgram())
		{
			Premake.WriteGroupEnd();
		}
	}

	public override void SetupRules(in List<CBuildItem> Others)
	{
		// List<CProgram> Programs = Others.OfType<CProgram>().ToList();
	}
	public sealed override void DefaultSetupRules()
	{
		Rules.FileDirectories.AddRange(new[]
		{
			CPath.FlareCombine(GetRootPath(), "**.cs")
		});
		Rules.Dependencies.Add("System");
		Rules.Defines.Add("DEBUG");
	}

	private bool IsBuildRulesProgram()
	{
		return ScriptFilePath.EndsWith("BuildRules.Build.cs");
	}
}
