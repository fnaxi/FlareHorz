// CopyRight © FlareHorz Team. All Rights Reserved.

using FlareBuildTool.Configuration;

public class Launch : FlareModuleRules
{
	public Launch()
	{
		dependencies.exposed.AddRange(
		[
			"Core"
		]);
		
		dependencies.hidden.AddRange(
		[
			// ...
		]);
	}
}
