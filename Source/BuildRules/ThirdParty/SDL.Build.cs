// CopyRight FlareHorz Team. All Rights Reserved.

using FlareBuildTool;
using FlareCore;

namespace BuildRules.ThirdParty;

public class CSDL : CThirdPartyModuleRules
{
	public CSDL()
	{
		OutputType = EBuildOutputType.None;
		
		FileDirectories.AddRange(new[]
		{
			CPath.FlareCombine(CScript.ProjectPath, "include", "**.h"),
			CPath.FlareCombine(CScript.ProjectPath, "LICENSE.txt"),
			CPath.FlareCombine(CScript.ProjectPath, "README.md")
		});
		
		PublicIncludeDirectories.Add(CPath.FlareCombine(CScript.ProjectPath, "include"));
		PublicLibraryDirectories.Add(CPath.FlareCombine(CScript.ProjectPath, "lib", "x64"));
		
		PublicDependencyLibraries.AddRange(new[]
		{
			"SDL3"
		});
	}
}
