// CopyRight FlareHorz Engine Development Team. All Rights Reserved.

using FlareCore;

namespace FlareBuildTool.Solution;

/// <summary>
/// Target rules that are specific to selected configuration.
/// </summary>
[FlareBuildToolAPI]
public class FConfigurationTargetRules : FFlareObject
{
	/// <summary>
	/// Calls when object of that class is created.
	/// </summary>
	protected override void OnObjectCreated()
	{
		base.OnObjectCreated();
		
		Defines = new List<string>();
	}
	
	/// <summary>
	/// Defines this configuration have.
	/// </summary>
	public List<string> Defines;
	
	/// <summary>
	/// For which configuration this rules is.
	/// </summary>
	public EConfiguration ForWhichConfiguration;
}
