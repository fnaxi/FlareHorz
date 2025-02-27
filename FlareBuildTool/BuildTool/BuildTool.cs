// CopyRight FlareHorz Engine Development Team. All Rights Reserved.

using FlareCore;
using FlareCore.Logger;

namespace FlareBuildTool.BuildTool;

/// <summary>
/// Build tool class.
/// Combines all actions needed to be done to build entire solution.
/// Actually the scheme of building generally looks like this:
/// Solution:
///		Target (Project)
///			Module
/// </summary>
public class FBuildTool : FFlareObject
{
	/// <summary>
	/// Sets default values.
	/// </summary>
	public FBuildTool()
	{
	}
	
	/// <summary>
	/// Run the build process.
	/// <param name="ArgumentsCount">Count of arguments.</param>
	/// <param name="Arguments">Console-line arguments.</param>
	/// </summary>
	public Int32 GuardedMain(Int32 ArgumentsCount, string[] Arguments)
	{
		return 0;
	}
}
