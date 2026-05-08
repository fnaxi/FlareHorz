// CopyRight FlareHorz Team. All Rights Reserved.

using FlareBuildTool;

namespace BuildRules.Runtime;

public class CEngine : CModuleRules
{
	public CEngine()
	{
		// TODO: can't use SDL as a private dependency, find out why
		PublicDependencyModules.AddRange(new []
		{
			"Core",
			"SDL",
			"Editor"
		});
	}
}
