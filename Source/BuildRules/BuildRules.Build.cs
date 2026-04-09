// CopyRight FlareHorz Team. All Rights Reserved.

global using static FlareCore.CAssert;
global using static FlareCore.CGlobal;
global using static FlareCore.CUtils;

using FlareBuildTool;
using FlareCore;

namespace BuildRules;

public class CBuildRules : CProgramRules
{
	public CBuildRules()
	{
		OutputType = EBuildOutputType.DynamicLibrary;
		
		Dependencies.Add($"{FlareCoreAssembly.GetName().Name}.dll");
		Dependencies.Add($"{TargetAssembly.GetName().Name}.exe");
	}
}
