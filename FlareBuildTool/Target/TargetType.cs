// CopyRight FlareHorz Team. All Rights Reserved.

namespace FlareBuildTool.Target;

/// <summary>
/// Type of the target.
/// </summary>
[FlareBuildToolAPI]
public enum ETargetType
{
	None,
	
	/// <summary>
	/// A target that builds to .exe (console app).
	/// </summary>
	Executable,
	
	/// <summary>
	/// A target that builds into .lib (static lib).
	/// </summary>
	StaticLibrary,
	
	/// <summary>
	/// A target that builds into .dll (dynamic lib).
	/// </summary>
	DynamicLibrary
}
