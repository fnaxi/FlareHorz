// CopyRight © FlareHorz Team. All Rights Reserved.

using System.Diagnostics;
using FlareBuildTool.Configuration;
using FlareCore;
using Microsoft.VisualStudio.SolutionPersistence.Model;
using Microsoft.VisualStudio.SolutionPersistence.Serializer;
using NDepend.Path;

namespace FlareBuildTool.ProjectFiles;

public class SolutionFileGenerator
{
	public async Task Create()
	{
		foreach (string platform in FlareBuildTool.platforms)
		{ solution.AddPlatform(platform); }
		
		foreach (string conf in FlareBuildTool.configurations)
		{ solution.AddBuildType(conf); }
		
		SolutionFolderModel solution_items = solution.AddFolder("/SolutionItems/");
		foreach (string file in FlareBuildTool.solution_files)
		{
			solution_items.AddFile(file);
		}
		
		foreach (FlareItem item in FlareBuildTool.items)
		{
			string folder_name = item is FlareModule mod ? $"/{mod.type}/" : "/Tools/";
			
			solution.AddProject(item.ms_file.Get(), item.GetMsType().ToString(), GetOrCreateSolutionFolder(folder_name));
			
			Logger.Get().LogDebug($"Added {item.GetDisplayName()} {item.ms_file.GetRelativePathFrom(Global.engine.source_path)} to the solution");
		}

		IAbsoluteFilePath sln_path = Global.engine.root_path.GetChildFileWithName($"{Global.solution_name}.sln");
		{
			await SolutionSerializers.SlnFileV12.SaveAsync(sln_path.Get(), solution, CancellationToken.None);

			Logger.Get().LogInformation($"Created {sln_path.GetRelativePathFrom(Global.engine.root_path)} solution file");
		}
	}

	private readonly SolutionModel solution = new();

	private SolutionFolderModel GetOrCreateSolutionFolder(string folder_name)
	{
		Debug.Assert(solution != null);
		
		SolutionFolderModel? folder = solution.FindFolder(folder_name);
		if (folder == null)
		{
			folder = solution.AddFolder(folder_name);
		}

		return folder;
	}
}
