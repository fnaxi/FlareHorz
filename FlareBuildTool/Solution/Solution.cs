// CopyRight FlareHorz Engine Development Team. All Rights Reserved.

using FlareBuildTool.Target;
using FlareCore;

namespace FlareBuildTool.Solution;

/// <summary>
/// A representation of solution we need to parse and build.
/// Contains info about all targets.
/// </summary>
[FlareBuildToolAPI]
public class FSolution
{
	/// <summary>
	/// Sets default values.
	/// </summary>
	public FSolution()
	{
		LuaFile = new FFile();
	}
	
	/// <summary>
	/// Lua file of this solution.
	/// </summary>
	private FFile LuaFile;
}
