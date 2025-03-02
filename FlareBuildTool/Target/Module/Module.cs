// CopyRight FlareHorz Engine Development Team. All Rights Reserved.

namespace FlareBuildTool.Target.Module;

/// <summary>
/// Every target have bunch of modules in Source/ directory.
/// We find all directories in Source/ folder and create {ModuleName}_API macro for each.
/// </summary>
[FlareBuildToolAPI]
public class FModule
{
	/// <summary>
	/// Sets default values.
	/// </summary>
	public FModule()
	{
	}
	
	/// <summary>
	/// Name of this module.
	/// </summary>
	public string Name;

	/// <summary>
	/// Followed this style: "{ModuleName}_API".
	/// </summary>
	public string APIMacro;
}
