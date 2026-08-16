// CopyRight © FlareHorz Team. All Rights Reserved.

using System;
using System.Linq;

namespace FlareBuildTool.Core;

/**
 * An exception that will return a unique exit code.
 */
public class BuildException(BuildResult in_result, string? message = null) 
	: Exception(message)
{
	public BuildResult result { get; } = in_result;
}

/**
 * Extension methods for build exceptions.
 */
public static class BuildExceptionExtensions
{
	/** Get the BuildResult for a provided Exception. */
	public static BuildResult GetBuildResult(this Exception ex)
	{
		return (ex as BuildException)?.result
				?? (ex.InnerException as BuildException)?.result
				?? (ex as AggregateException)?.InnerExceptions.OfType<BuildException>().FirstOrDefault()?.result
				?? BuildResult.CompilationError;
	}
}
