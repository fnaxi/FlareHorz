// CopyRight FlareHorz Engine Development Team. All Rights Reserved.

using FlareCore;

namespace FlareBuildTool.Solution;

/// <summary>
/// Solution we open to build Dark Build Tool project and then open full one.
/// <remarks>Actually FlareHorz.sln.MinimalSolution.lua exists in repository.</remarks>
/// </summary>
[FlareBuildToolAPI]
public class FMinimalSolution : FFlareObject
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
	/// Lua file of minimal solution.
	/// </summary>
	private FFile LuaFile;
}
