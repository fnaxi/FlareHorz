// CopyRight FlareHorz Team. All Rights Reserved.

using FlareCore;

namespace FlareBuildTool;

/// <summary>
/// Flare Build Tool API.
/// </summary>
public class FlareBuildToolAPI : API
{
	/// <summary>
	/// Get API string.
	/// </summary>
	public override string GetName()
	{
		return "FlareBuildTool";
	}
}

/// <summary>
/// API for .Target.cs and .Build.cs files.
/// </summary>
public class BuildRulesAPI : API
{
	/// <summary>
	/// Get API string.
	/// </summary>
	public override string GetName()
	{
		return "BuildRules";
	}
}
