// CopyRight FlareHorz Team. All Rights Reserved.

using FlareBuildTool;

namespace BuildRules.Programs;

public class CFlareCore : CProgramRules
{
	public CFlareCore()
	{
		OutputType = EBuildOutputType.DynamicLibrary;
	}
}
