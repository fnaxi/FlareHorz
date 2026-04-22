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
		const string BuildRulesFile = "BuildRules.csproj";
		const string Configuration  = "Debug";
		const string Architecture   = "x64";
		const string Verbosity      = "minimal"; // quiet/minimal/normal/detailed/diagnostic
		ExecuteConsoleCommand($"dotnet build {CPath.FlareCombine(BuildRulesPath, BuildRulesFile)} -c {Configuration} -a {Architecture} -v {Verbosity}");
		
		CAction.Run(true, () =>
		{
			foreach (string GroupPath in Directory.GetDirectories(BuildRulesPath).Where(s => !s.EndsWith("Programs")))
			{
				Verify(Enum.TryParse(Path.GetFileName(GroupPath), false, out EModuleType ModuleType));

				foreach (string ScriptFilePath in Directory.GetFiles(GroupPath))
				{
					VerifyScriptFile(ScriptFilePath);

					string Name = CPath.GetFileNameWithoutDoubleExtension(ScriptFilePath);
					BuildItems.Add(Name, new CModule(Name, ScriptFilePath, ModuleType));
				}
			}

			return $"Found {GetModulesCount()} module{(GetModulesCount() > 1 ? "s" : "")}";
		});
		
		CAction.Run(true, () =>
		{
			foreach (string ScriptFilePath in Directory.GetFiles(CPath.Combine(BuildRulesPath, "Programs")))
			{
				VerifyScriptFile(ScriptFilePath);

				string Name = CPath.GetFileNameWithoutDoubleExtension(ScriptFilePath);
				BuildItems.Add(Name, new CProgram(Name, ScriptFilePath));
			}

			BuildItems.Add("BuildRules", new CProgram("BuildRules", CPath.Combine(BuildRulesPath, "BuildRules.Build.cs")));

			return $"Found {GetProgramsCount()} program{(GetProgramsCount() > 1 ? "s" : "")}";
		});

		CAction.Run(true, () =>
		{
			foreach (CBuildItem BuildItem in BuildItems.Values)
			{
				BuildItem.SetupRules(BuildItems);
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
			"Win64", "x64"
		});

		foreach (CBuildItem BuildItem in BuildItems.Values)
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
		return BuildItems.Values.OfType<CModule>().Count();
	}
	private Int32 GetProgramsCount()
	{
		return BuildItems.Values.OfType<CProgram>().Count();
	}

	/** Represent a list of all known build items with name as a key. */
	private Dictionary<string, CBuildItem> BuildItems = new();
	
	/** A list of additional files to include in the solution . */
	private List<string> SolutionItems = new() { "README.md", ".gitignore" };
	
	private CPremakeFileHandle PremakeFile = new();
}

public abstract class CEntryPoint
{
	private static Int32 Main(string[] Arguments)
	{
		return new CFlareBuildTool().Run(Arguments, Assembly.GetExecutingAssembly());
	}
}

