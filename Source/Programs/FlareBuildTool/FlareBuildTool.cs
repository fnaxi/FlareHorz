// CopyRight FlareHorz Team. All Rights Reserved.

global using static FlareCore.CAssert;
global using static FlareCore.CGlobal;
global using static FlareCore.CUtils;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FlareCore;

namespace FlareBuildTool;

public class CFlareBuildTool : CApplication
{
	protected override Int32 GuardedMain()
	{
		CAction.Run(true, () =>
		{
			foreach (string GroupPath in Directory.GetDirectories(BuildRulesPath).Where(s => !s.EndsWith("Programs")))
			{
				Enum.TryParse(Path.GetFileName(GroupPath), false, out EModuleType ModuleType);

				foreach (string ScriptFilePath in Directory.GetFiles(GroupPath))
				{
					VerifyScriptFile(ScriptFilePath);

					BuildItems.Add(new CModule(ScriptFilePath, ModuleType));
				}
			}

			return $"Found {GetModulesCount()} module{(GetModulesCount() > 1 ? "s" : "")}";
		});
		
		CAction.Run(true, () =>
		{
			foreach (string ScriptFilePath in Directory.GetFiles(CPath.Combine(BuildRulesPath, "Programs")))
			{
				VerifyScriptFile(ScriptFilePath);

				BuildItems.Add(new CProgram(ScriptFilePath));
			}

			BuildItems.Add(new CProgram(CPath.Combine(BuildRulesPath, "BuildRules.Build.cs")));

			return $"Found {GetProgramsCount()} program{(GetProgramsCount() > 1 ? "s" : "")}";
		});

		CAction.Run(true, () =>
		{
			foreach (CBuildItem BuildItem in BuildItems)
			{
				BuildItem.SetupRules( BuildItems.Where(item => item != BuildItem).ToList() );
			}
			
			return "Build rules have been setup";
		});
		
		CAction.Run(true, () =>
		{
			CreatePremakeFile();
			return "Created premake file";
		});
		
		return RunPremake("vs2022");
	}
	
	private void CreatePremakeFile()
	{
		const string PremakeFileName = SolutionName + ".lua";
		string PremakeFilePath = Path.Combine(IntermediatePath, PremakeFileName);
		
		PremakeFile.CreateOrLoad(PremakeFilePath);
		PremakeFile.Clear();
		
		GeneratePremakeFile();
		
		PremakeFile.Close();
	}
	
	/** Generate code inside the premake file. */
	private void GeneratePremakeFile()
	{
		PremakeFile.WriteCopyright();
		
		PremakeFile.WriteRequire("vstudio");
		PremakeFile.WriteRegisterSolutionFolder("GlobalItems");
		
		PremakeFile.WriteWorkspace(SolutionName);
		PremakeFile.WriteLocation(CPath.ToFlare(SolutionPath));
		
		PremakeFile.WriteSolutionFolder("GlobalItems", SolutionItems);
		
		PremakeFile.WriteArchitecture("x64");
		
		PremakeFile.WriteConfigurations(new List<string>
		{
			"Debug", "Development", "Shipping"
		});
		PremakeFile.WritePlatforms(new List<string>
		{
			"Win64",
			
			// To allow FlareBuildTool rebuild BuildRules on PreBuildCommands step
			"x64"
		});

		foreach (CBuildItem BuildItem in BuildItems)
		{
			BuildItem.GeneratePremakeCode(PremakeFile);
		}
	}
	
	/** Generate project files such as .sln, .csproj, etc. using premake. */
	private Int32 RunPremake(string InBuildOption)
	{
		string PremakeExecutablePath = Path.Combine(BinariesPath, "premake5.exe");
		if (!File.Exists(PremakeExecutablePath))
		{
			CLog.Error("Premake executable not found! Run GenerateProjectFiles.bat script to set up the project.");
		}
		
		return ExecuteConsoleCommand($"{PremakeExecutablePath} {InBuildOption} --file=\"{PremakeFile.GetFullPath()}\"");
	}

	private void VerifyScriptFile(string InScriptFilePath)
	{
		Verify(InScriptFilePath.EndsWith(".Build.cs"), "Build files should have '.Build.cs' extension!");
	}
	
	private Int32 GetModulesCount()
	{
		return BuildItems.OfType<CModule>().Count();
	}
	private Int32 GetProgramsCount()
	{
		return BuildItems.OfType<CProgram>().Count();
	}

	private List<CBuildItem> BuildItems = new();
	private CPremakeFileHandle PremakeFile = new();
	
	/** A list of additional files to include in the solution . */
	private List<string> SolutionItems = new() { "README.md", ".gitignore" };
}

public abstract class CEntryPoint
{
	private static Int32 Main(string[] Arguments)
	{
		return new CFlareBuildTool().Run(Arguments, Assembly.GetExecutingAssembly());
	}
}

