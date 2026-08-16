// CopyRight © FlareHorz Team. All Rights Reserved.

#if NDEBUG
	#error FlareBuildTool must be built only in Debug configuration!
#endif

using System.Diagnostics;
using System.Reflection;
using FlareBuildTool.Configuration;
using FlareBuildTool.Core;
using FlareBuildTool.Modes;
using FlareCore;
using NDepend.Path;

namespace FlareBuildTool;

internal class FlareBuildTool : Application
{
	protected override int GuardedMain()
	{
		Debug.Assert(configurations.Count != 0, "No configurations are specified!");
		Debug.Assert(platforms.Count != 0, "No platforms are specified!");
		
		SingleInstanceMutex? mutex = null;
		try
		{
			// Acquire a lock
			if (!b_no_mutex)
			{
				string mutex_name = SingleInstanceMutex.GetUniqueMutexForPath("FlareBuildTool_Mutex", (Assembly.GetExecutingAssembly().Location).ToAbsoluteDirectoryPath());
				mutex = new SingleInstanceMutex(mutex_name, b_wait_mutex);
			}

			Type? mode_type = typeof(BuildMode); // default mode
			
			// Try to get the correct mode
			if (mode_name != null && !m_mode_name_to_type.TryGetValue(mode_name, out mode_type))
			{
				List<string> modes = m_mode_name_to_type.Keys.ToList();
				modes.Sort(StringComparer.OrdinalIgnoreCase);

				Logger.Get().LogError($"No mode named {mode_type}. Available modes are:\n  {String.Join("\n  ", modes)}");
				return (int)BuildResult.Unknown;
			}
			
			// Search for build rules
			List<IAbsoluteFilePath> rules = Utils.Concat(DiscoverModules([Global.engine.rules_path]), DiscoverCSharpProjects([Global.engine.tools_path]));
			foreach (IAbsoluteFilePath file in rules)
			{
				items.Add(file.FileExtension == ".csproj" ? new FlareTool(file) : new FlareModule(file));
			}
			
			Logger.Get().LogInformation(GetModules().Count != 0 ? $"Found {GetModules().Count} C++ module(s)" : "No modules were found!");
			Logger.Get().LogInformation(GetTools().Count != 0 ? $"Found {GetTools().Count} C# utility tool(s)" : "No tools were found!");

			// Create the appropriate handler
			ToolMode mode = (ToolMode)Activator.CreateInstance(mode_type)!;
			
			// Execute the mode
			Logger.Get().LogInformation($"Executing {mode_name} tool mode...");
			return mode.ExecuteAsync().GetAwaiter().GetResult();
		}
		catch (Exception ex)
		{
			Logger.Get().LogError($"Exception was thrown by {ex.Source} in method: {ex.TargetSite}");
			Logger.Get().LogError($"Message: {ex.Message}");
			if (ex.StackTrace != null)
			{
				Logger.Get().LogError("Stack trace:");
				foreach (string trace in ex.StackTrace.Split('\n'))
				{
					Logger.Get().LogError(trace);
				}
			}

			return ex is BuildException ? (int)ex.GetBuildResult() : 1;
		}
		finally
		{
			mutex?.Dispose();
			Logger.Get().LogTrace("Mutex has been disposed");
		}
	}
	
	public static List<FlareItem> items = [];
	
	public static readonly List<string> platforms = ["x64"];
	public static readonly List<string> configurations = ["Debug", "Release"];
	public static readonly List<string> solution_files = [".gitignore", "README.md"];
	
	public static List<FlareModule> GetModules() => items.OfType<FlareModule>().ToList();
	public static List<FlareTool> GetTools() => items.OfType<FlareTool>().ToList();
	
	private List<IAbsoluteFilePath> DiscoverCSharpProjects(List<IAbsoluteDirectoryPath> search_paths)
		=> Discover(search_paths, "*.csproj");
	
	private List<IAbsoluteFilePath> DiscoverModules(List<IAbsoluteDirectoryPath> search_paths)
		=> Discover(search_paths, "*.flare.cs");
	
	private List<IAbsoluteFilePath> Discover(List<IAbsoluteDirectoryPath> search_paths, string filter)
	{
		foreach (IAbsoluteDirectoryPath path in search_paths)
		{
			Debug.Assert( path.Exists, $"Search path {path.Get()} does not exist!" );
		}

		List<string> files = [];
		foreach (IAbsoluteDirectoryPath path in search_paths)
		{
			files.AddRange(Directory.GetFiles(path.Get(), filter, SearchOption.AllDirectories));
		}

		return new(files.Select(x => x.ToAbsoluteFilePath()));
	}
	
	/** The tool mode to execute. */
	[CommandLine("-Mode?", description = "Select tool mode. One of the following (default tool mode is \"Build\"): GenerateProjectFiles, Build.")]
	[CommandLine("-ProjectFiles", value = "GenerateProjectFiles", description = "Generate Visual Studio project files. Equivalent to -Mode?GenerateProjectFiles.")]
	// todo: [CommandLine("-Build", value = "Build")]
	// todo: [CommandLine("-Clean", value = "Clean", Description = "Clean build products. Equivalent to -Mode=Clean")]
	private static string? mode_name = null;
	
	/** Whether to ignore the mutex. */
	[CommandLine("-NoMutex", description = "Allow more than one instance of the program to be run at once.")]
	private static bool b_no_mutex = false;

	/** Whether to wait for the mutex rather than aborting immediately. */
	[CommandLine("-WaitMutex", description = "Wait for another instance to finish and then start, rather than aborting immediately.")]
	private static bool b_wait_mutex = false;
	
	/** Get all the valid tool modes. */
	private static Dictionary<string, Type> GetModes()
	{
		Dictionary<string, Type> mode_name_to_type = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
		foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
		{
			if (!type.IsClass || type.IsAbstract || !type.IsSubclassOf(typeof(ToolMode))) continue;
			
			ToolModeAttribute? attribute = type.GetCustomAttribute<ToolModeAttribute>();
			Debug.Assert(attribute != null, $"Class '{type.Name}' should have a ToolModeAttribute");
			
			mode_name_to_type.Add(attribute.name, type);
		}
		return mode_name_to_type;
	}
	private readonly Dictionary<string, Type> m_mode_name_to_type = GetModes();
}

internal abstract class EntryPoint
{
	private static int Main(string[] arguments)
	{
		return new FlareBuildTool().Run(arguments, Assembly.GetExecutingAssembly());
	}
}
