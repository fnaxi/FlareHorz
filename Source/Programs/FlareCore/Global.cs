// CopyRight FlareHorz Team. All Rights Reserved.

global using static FlareCore.CAssert;
global using static FlareCore.CGlobal;
global using static FlareCore.CUtils;

using System.Reflection;

namespace FlareCore;

/**
 * Global configuration settings and paths that are used across the application.
 */
public static class CGlobal
{
	public static Assembly TargetAssembly;
	public static Assembly CoreAssembly;

	public const string CopyRightNotice = "CopyRight FlareHorz Team. All Rights Reserved.";
	
	public const string SolutionName = "FlareHorz";
	public static string SolutionPath;

	public static string BinariesPath;
	public static string IntermediatePath;
	
	public static string SourcePath;
	public static string BuildRulesPath;
	
	public static string SavedPath;
	public static string LogsPath;
}
