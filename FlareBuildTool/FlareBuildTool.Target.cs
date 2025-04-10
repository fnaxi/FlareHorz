// CopyRight FlareHorz Team. All Rights Reserved.

using System;
using FlareBuildTool;
using FlareBuildTool.Target;

/// <summary>
/// Define rules for FlareBuildTool target.
/// </summary>
[BuildRulesAPI]
class FFlareBuildToolTarget : FTargetRules
{
	/// <summary>
	/// Calls when object of that class is created.
	/// </summary>
	protected override void OnObjectCreated()
	{
		base.OnObjectCreated();

		TargetType = ETargetType.Executable;
		TargetLanguage = ETargetLanguage.CS;
		bStartupTarget = false;
		
		Links.AddRange(new string[]
		{
			"System", "FlareCore"
		});

		Files.AddRange(new string[]
		{
			"/FlareBuildTool/**.lua" 
		});

		Group = "Programs";
	}
}
