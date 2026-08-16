// CopyRight © FlareHorz Team. All Rights Reserved.

using FlareBuildTool.Configuration;
using FlareBuildTool.Core;
using FlareBuildTool.ProjectFiles;
using FlareCore;

namespace FlareBuildTool.Modes;

[ToolMode("GenerateProjectFiles")]
class GenerateProjectFilesMode : ToolMode
{
	public override async Task<int> ExecuteAsync()
	{
		await solution_generator.Create();
		
		foreach (FlareModule mod in FlareBuildTool.GetModules())
		{
			project_generator.Create(Global.engine.source_path.GetChildDirectoryWithName($"{mod.type}/{mod.name}"), mod);
		}
		
		return (int)BuildResult.Succeeded;
	}
	
	private ProjectFileGenerator project_generator = new();
	private SolutionFileGenerator solution_generator = new();
}
