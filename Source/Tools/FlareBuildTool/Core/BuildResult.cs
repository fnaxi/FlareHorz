// CopyRight © FlareHorz Team. All Rights Reserved.

namespace FlareBuildTool.Core;

/** Return codes used by Flare Build Tool. */
public enum BuildResult
{
	/** Build succeeded. */
	Succeeded = 0,
	
	/** Unknown error. */
	Unknown = 1,
	
	/** All targets were up to date. */
	UpToDate = 2,
	
	/** Build was canceled, this is used on the engine side only. */
	Canceled = 3,
	
	/** The process has most likely crashed. This is what editor returns in case of an assert. */
	CrashOrAssert = 4,
	
	/** Build failed due to compilation errors. */
	CompilationError = 5,
	
	/** Build failed due to flare.cs files errors. */
	RulesError = 6,
	
	/** Another instance of UBT was already running. */
	ConflictingInstance = 7
}

/** Helper extensions for BuildResult. */
public static class BuildResultExtensions
{
	/** Test to see if the return code is a success. */
	public static bool Succeeded(this BuildResult result)
		=> result is BuildResult.Succeeded or BuildResult.UpToDate;
}
