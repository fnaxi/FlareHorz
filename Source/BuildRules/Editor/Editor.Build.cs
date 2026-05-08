// CopyRight FlareHorz Team. All Rights Reserved.

using FlareBuildTool;

namespace BuildRules.Editor;

public class CEditor : CModuleRules
{
	public CEditor()
	{
		PublicDependencyModules.AddRange(new []
		{
			"Core",
			"ImGui"
		});
		
		PrivateDependencyModules.AddRange(new []
		{
			"SDL"
		});
	}
}
