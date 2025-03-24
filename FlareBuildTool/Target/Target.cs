// CopyRight FlareHorz Engine Development Team. All Rights Reserved.

using FlareBuildTool.Target.Module;
using FlareCore;

namespace FlareBuildTool.Target;

/// <summary>
/// A class that describes one project in the solution.
/// </summary>
[FlareBuildToolAPI]
public class FTarget : FFlareObject
{
	/// <summary>
	/// Calls when object of that class is created.
	/// </summary>
	protected override void OnObjectCreated()
	{
		base.OnObjectCreated();
		
		TargetCsFile = CreateObject<FFile>("TargetCsFile");
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
