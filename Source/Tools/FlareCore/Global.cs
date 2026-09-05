// CopyRight FlareHorz Team. All Rights Reserved.

using System.Diagnostics;
using System.IO;
using NDepend.Path;

namespace FlareCore;

public static class Global
{
	public const string solution_name = "FlareHorz";
	public const string rules_project_name = "FlareBuildRules";
	
	public static readonly ProjectPaths engine = new(GetSolutionPath());
	
	//@TODO: Game paths
	// public static readonly ProjectPaths game = new();
	
	public class ProjectPaths
	{
		public ProjectPaths(IAbsoluteDirectoryPath in_root_path)
		{
			root_path = in_root_path;
			
			Debug.Assert(root_path.Exists);
			Debug.Assert(root_path.Get().EndsWith(solution_name), "Failed to get root path!");
			
			//@TODO: Revisit this
			// Directory.SetCurrentDirectory(root_path.Get());
			
			binaries_path		= root_path.GetChildDirectoryWithName("Binaries");
			intermediate_path	= root_path.GetChildDirectoryWithName("Intermediate");
			
			source_path			= root_path.GetChildDirectoryWithName("Source");
			
			tools_path			= root_path.GetChildDirectoryWithName($"Source/Tools");
			rules_path			= root_path.GetChildDirectoryWithName($"Source/Tools/{rules_project_name}");

			saved_path			= root_path.GetChildDirectoryWithName("Saved");
			logs_path			= root_path.GetChildDirectoryWithName("Saved/Logs");
		}

		public readonly IAbsoluteDirectoryPath root_path;

		public readonly IAbsoluteDirectoryPath binaries_path;
		public readonly IAbsoluteDirectoryPath intermediate_path;

		public readonly IAbsoluteDirectoryPath source_path;
		public readonly IAbsoluteDirectoryPath tools_path;
		public readonly IAbsoluteDirectoryPath rules_path;

		public readonly IAbsoluteDirectoryPath saved_path;
		public readonly IAbsoluteDirectoryPath logs_path;
	}
	
	/** Gets solution root directory. */
	private static IAbsoluteDirectoryPath GetSolutionPath()
	{
		string current_directory = Directory.GetCurrentDirectory();

		if (!current_directory.EndsWith(solution_name)) //@TODO: Revisit this
		{
			current_directory = PathUtils.GetParent(current_directory);
		}

		return current_directory.ToAbsoluteDirectoryPath();
	}
}
