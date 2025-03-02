// CopyRight FlareHorz Engine Development Team. All Rights Reserved.

namespace FlareBuildTool.Target;

/// <summary>
/// Target rules that are specific to selected configuration.
/// </summary>
[FlareBuildToolAPI]
public class FConfigurationTargetRules
{
	/// <summary>
	/// Sets default values.
	/// </summary>
	public FConfigurationTargetRules()
	{
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
