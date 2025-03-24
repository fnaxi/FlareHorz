// CopyRight FlareHorz Engine Development Team. All Rights Reserved.

using FlareBuildTool.Target;
using FlareCore;
using FlareCore.Logger;

namespace FlareBuildTool.Solution;

/// <summary>
/// A representation of solution we need to parse and build.
/// Contains info about all targets.
/// </summary>
[FlareBuildToolAPI]
public class FSolution : FFlareObject
{
	/// <summary>
	/// Calls when object of that class is created.
	/// </summary>
	protected override void OnObjectCreated()
	{
		base.OnObjectCreated();
		
		LuaFile = CreateObject<FFile>("LuaFile");
	}
	
	/// <summary>
	/// Lua file of this solution.
	/// </summary>
	private FFile LuaFile;
}
