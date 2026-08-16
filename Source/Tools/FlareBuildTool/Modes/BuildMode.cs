// CopyRight © FlareHorz Team. All Rights Reserved.

using System.Threading.Tasks;
using FlareBuildTool.Core;
using FlareCore;

namespace FlareBuildTool.Modes;

[ToolMode("Build")]
class BuildMode : ToolMode
{
	public override Task<int> ExecuteAsync()
	{
		Logger.Get().LogInformation("Building C++ files...");
		
		return Task.FromResult((int)BuildResult.Succeeded);
	}
}
