// CopyRight FlareHorz Engine Development Team. All Rights Reserved.

using FlareCore;

namespace FlareBuildTool;

/// <summary>
/// Flare Build Tool API.
/// </summary>
public class FlareBuildToolAPI : API
{
	/// <summary>
	/// Sets default values.
	/// </summary>
	public FlareBuildToolAPI()
	{ }
	
	/// <summary>
	/// Get API string. Should be overrided.
	/// </summary>
	public override string GetName()
	{
		return "FlareBuildToolAPI";
	}
}
