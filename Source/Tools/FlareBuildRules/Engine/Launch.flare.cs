// CopyRight © FlareHorz Team. All Rights Reserved.

using FlareBuildTool.Configuration;

public class Launch : FlareModuleRules
{
	public Launch()
	{
		exposed_dependencies.AddRange(
		[
			"Core"
		]);
		
		hidden_dependencies.AddRange(
		[
			// ...
		]);
	}
}
