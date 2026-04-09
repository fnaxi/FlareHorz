// CopyRight FlareHorz Team. All Rights Reserved.

using FlareBuildTool;
using FlareCore;

namespace BuildRules.Programs;

public class CFlareBuildTool : CProgramRules
{
	public CFlareBuildTool()
	{
		OutputType = EBuildOutputType.Executable;
		
		Dependencies.Add("FlareCore");

		const string BuildRulesFile = "BuildRules.csproj";
		const string Configuration  = "Debug";
		const string Architecture   = "x64";
		const string Verbosity      = "normal"; // quiet/minimal/normal/detailed/diagnostic
		PreBuildCommands.Add($"dotnet build {CPath.FlareCombine(BuildRulesPath, BuildRulesFile)} -c {Configuration} -a {Architecture} -v {Verbosity}");
	}
}
