// CopyRight FlareHorz Team. All Rights Reserved.

using FlareCore;

namespace FlareBuildTool;

/// <summary>
/// The entry point of the application.
/// </summary>
[FlareBuildToolAPI]
public class FGlobal : FFlareObject
{
	/// <summary>
	/// Solution path.
	/// </summary>
	public static string SolutionPath;

	/// <summary>
	/// Name of that solution.
	/// </summary>
	public static string SolutionName;
	
	/// <summary>
	/// Current configuration.
	/// </summary>
	public static string Configuration = 
#if FH_DEBUG
		"Debug";
#elif FH_DEVELOPMENT
		"Development";
#else
		#error Unknown configuration
#endif
}
