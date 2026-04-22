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
	}
}
