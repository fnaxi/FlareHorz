// CopyRight FlareHorz Team. All Rights Reserved.

using System.Collections.Generic;
using FlareCore;

namespace FlareBuildTool.Solution;

/// <summary>
/// Target rules that are specific to configuration.
/// </summary>
[FlareBuildToolAPI]
public class FConfRules : FFlareObject
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
}
