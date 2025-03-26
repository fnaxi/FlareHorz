// CopyRight FlareHorz Team. All Rights Reserved.

using System;
using FlareBuildTool;
using FlareBuildTool.Target;

/// <summary>
/// Define rules for FlareBuildTool target.
/// </summary>
[FlareBuildToolAPI]
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
		
		Links.AddRange(new string[] { "System", "FlareCore" });

		Group = "Programs";
	}
}
