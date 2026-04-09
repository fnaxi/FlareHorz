// CopyRight FlareHorz Team. All Rights Reserved.

using FlareBuildTool;

namespace BuildRules.Runtime;

public class CLaunch : CModuleRules
{
	public CLaunch()
	{
		bStartup = true;
		
		PublicDependencyModules.AddRange(new[]
		{
			"Core"
		});
	}
}
