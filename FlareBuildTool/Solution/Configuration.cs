// CopyRight FlareHorz Engine Development Team. All Rights Reserved.

namespace FlareBuildTool.Solution;

/// <summary>
/// Configuration. Should always be same as real configuration name.
/// </summary>
[FlareBuildToolAPI]
public enum EConfiguration
{
	None,
	
	/// <summary>
	/// Debug configuration.
	/// </summary>
	Debug,
	
	/// <summary>
	/// Development configuration.
	/// </summary>
	Development,
	
	/// <summary>
	/// Shipping configuration.
	/// </summary>
	Shipping
}
