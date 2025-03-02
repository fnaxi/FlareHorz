// CopyRight FlareHorz Engine Development Team. All Rights Reserved.

using FlareBuildTool.Target.Module;
using FlareCore;

namespace FlareBuildTool.Target;

/// <summary>
/// A class that describes one project in the solution.
/// </summary>
[FlareBuildToolAPI]
public class FTarget
{
	/// <summary>
	/// Sets default values.
	/// </summary>
	public FTarget()
	{
		TargetCsFile = new FFile();
		Modules = new List<FModule>();
	}
	
	/// <summary>
	/// Name of this target.
	/// </summary>
	public string Name;

	/// <summary>
	/// .Target.cs file in root of this target's path. Required to create FTargetRules.
	/// </summary>
	private FFile TargetCsFile;

	/// <summary>
	/// Modules that target have.
	/// </summary>
	public List<FModule> Modules;
}
