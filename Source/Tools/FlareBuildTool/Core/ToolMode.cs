// CopyRight © FlareHorz Team. All Rights Reserved.

using System;
using System.Threading.Tasks;

namespace FlareBuildTool.Core;

/**
 * Attribute used to specify the name and options for a Flare Build Tool mode.
 */
[AttributeUsage(AttributeTargets.Class)]
internal sealed class ToolModeAttribute(string in_name) : Attribute
{
	/** Name of this mode. */
	public string name { get; } = in_name;
}

/**
* Base class for standalone Flare Build Tool modes.
* Different modes can be invoked using the -Mode?[Name] argument on the command line, where [Name] is determined by the ToolModeAttribute on a ToolMode derived class.
*/
abstract class ToolMode
{ 
	/**
	 * Entry point for this command.
	 * <returns>Exit code for the process</returns>
	 */
	public abstract Task<int> ExecuteAsync();
}
