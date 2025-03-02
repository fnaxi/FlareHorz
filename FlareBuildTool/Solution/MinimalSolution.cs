// CopyRight FlareHorz Engine Development Team. All Rights Reserved.

using FlareCore;

namespace FlareBuildTool.Solution;

/// <summary>
/// Solution we open to build Dark Build Tool project and then open full one.
/// <remarks>Actually FlareHorz.sln.MinimalSolution.lua exists in repository.</remarks>
/// </summary>
[FlareBuildToolAPI]
public class FMinimalSolution
{
	/// <summary>
	/// Sets default values.
	/// </summary>
	public FMinimalSolution()
	{
		LuaFile = new FFile();
	}

	/// <summary>
	/// Lua file of minimal solution.
	/// </summary>
	private FFile LuaFile;
}
