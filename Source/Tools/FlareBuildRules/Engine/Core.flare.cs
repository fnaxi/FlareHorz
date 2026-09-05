// CopyRight © FlareHorz Team. All Rights Reserved.

using FlareBuildTool.Configuration;

public class Core : FlareModuleRules
{
	public Core()
	{
		dependencies.exposed.AddRange(
		[
			// ...
		]);
		
		dependencies.hidden.AddRange(
		[
			// ...
		]);
		
		// You can use the following prefix to specify that something is only for specific configurations
		// exposed_include_dirs.Add("Debug+Development^^Exposed/SomeDirectory");
	}
}
