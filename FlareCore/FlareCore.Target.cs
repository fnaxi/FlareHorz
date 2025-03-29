// CopyRight FlareHorz Team. All Rights Reserved.

using System;
using FlareBuildTool;
using FlareBuildTool.Target;

/// <summary>
/// Define rules for FlareCore target.
/// </summary>
[FlareBuildToolAPI]
class FFlareCoreTarget : FTargetRules
{
	/// <summary>
	/// Calls when object of that class is created.
	/// </summary>
	protected override void OnObjectCreated()
	{
		base.OnObjectCreated();

		TargetType = ETargetType.DynamicLibrary;
		TargetLanguage = ETargetLanguage.CS;
		
		Links.AddRange(new string[]
		{
			"System"
		});

		Group = "Programs";
	}
}
