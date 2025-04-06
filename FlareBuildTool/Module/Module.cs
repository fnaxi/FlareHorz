// CopyRight FlareHorz Team. All Rights Reserved.

using FlareCore;

namespace FlareBuildTool.Module;

/// <summary>
/// Every target have bunch of modules in Source/ directory.
/// We find all directories in Source/ folder and create {ModuleName}_API macro for each.
/// </summary>
[FlareBuildToolAPI]
public class FModule : FFlareObject
{
	/// <summary>
	/// Calls when object of that class is created.
	/// </summary>
	protected override void OnObjectCreated()
	{
		base.OnObjectCreated(); 
		
		BuildCsFile = CreateObject<FFile>("BuildCsFile");
		ModuleRules = CreateObject<FModuleRules>("ModuleRules");
	}

	/// <summary>
	/// .Build.cs file of this module.
	/// </summary>
	public FFile BuildCsFile;

	/// <summary>
	/// Rules of this module.
	/// </summary>
	public FModuleRules ModuleRules;
	
	/// <summary>
	/// Name of this module.
	/// </summary>
	public string Name;

	/// <summary>
	/// Followed by this style: "{ModuleName}_API".
	/// </summary>
	public string APIMacro;
}
