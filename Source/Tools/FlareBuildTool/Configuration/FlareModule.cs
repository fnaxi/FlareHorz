// CopyRight © FlareHorz Team. All Rights Reserved.

using System.Diagnostics;
using System.Reflection;
using FlareCore;
using NDepend.Path;

namespace FlareBuildTool.Configuration;

public enum ModuleType
{
	Invalid = 0,
	
	Engine,
	Editor,
	ThirdParty,
	Game
}

/**
 * <remarks> Name "Module" conflicts with System.Reflection.Module </remarks>
 */
public class FlareModule : FlareItem
{
	public FlareModule(IAbsoluteFilePath in_rules_file)
	{
		rules_file = in_rules_file;
		
		name = PathUtils.GetFileNameWithoutDoubleExtension(rules_file.FileName);
		uid = GetUIDFromText(name);
		
		bool b_type_is_valid = Enum.TryParse(rules_file.ParentDirectoryPath.DirectoryName, out type);
		Debug.Assert(b_type_is_valid);
		
		ms_file = Global.engine.source_path.GetChildDirectoryWithName($"{type}/{name}").GetChildFileWithName($"{name}.vcxproj");
		
		path = ms_file.ParentDirectoryPath;
		
		exposed_path = path.GetChildDirectoryWithName("Exposed");
		if (!exposed_path.Exists) Directory.CreateDirectory(exposed_path.Get());
		
		hidden_path = path.GetChildDirectoryWithName("Hidden");
		if (!hidden_path.Exists) Directory.CreateDirectory(hidden_path.Get());

		header_files	= FindFiles("*.h", "header");
		source_files	= FindFiles("*.cpp", "source");
		rules			= GatherRules<FlareModuleRules>(name);
		
		DefaultSetupRules();
		
		Logger.Get().LogTrace($"{name}: UID: {uid.ToString().ToUpper()}");
		Logger.Get().LogInformation($"Mounted {name} module with {rules_file.GetRelativePathFrom(Global.engine.rules_path)}");
	}
	
	/** Defines the type of this module. */
	public readonly ModuleType type;
	
	/** The path to the "Exposed/" directory of the module. */
	private IAbsoluteDirectoryPath exposed_path { init; get; }
	
	/** The path to the "Hidden/" directory of the module. */
	private IAbsoluteDirectoryPath hidden_path { init; get; }
	
	/** The paths to header and source files of this module relative to Source/ directory. */
	public List<IRelativeFilePath> header_files { get; set; } = new();
	public List<IRelativeFilePath> source_files { get; set; } = new();
	
	/** The rules for this module. */
	public readonly FlareModuleRules rules;
	//@TODO: public readonly FlareModuleRules act_rules;
	
	/** The name of the .flare.cs file this item was created from. */
	private IAbsoluteFilePath rules_file { init; get; }

	public override Guid GetMsType() => Guid.Parse("8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942");
	public override string GetDisplayName() => "module";

	public static int cpp_standard = 17;
	
	private static T GatherRules<T>(string type_name)
	{
		IAbsoluteFilePath dll_path = Global.engine.binaries_path.GetChildFileWithName($"{Global.rules_project_name}.dll");
		Debug.Assert(dll_path.Exists, $"Can't find {dll_path}! Make sure Rules project is built and up to date.");
		
		Assembly rules_dll = Assembly.LoadFrom(dll_path.Get());
		
		Type? runtime_type = rules_dll.GetType(type_name) ?? rules_dll.GetType($"_{type_name}");
		Debug.Assert(runtime_type != null, $"Can't find {type_name} class!");
		Debug.Assert(runtime_type.IsSubclassOf(typeof(T)), $"{type_name} should inherit {typeof(T)}!");
		
		T? runtime_rules = (T?) Activator.CreateInstance(runtime_type);
		Debug.Assert(runtime_rules != null, $"Failed to cast RuntimeType to {typeof(T)}!");

		return runtime_rules;
	}

	private void DefaultSetupRules()
	{
		rules.defines.hidden.Add($"{name.ToUpper()}_EXPORTS");
		rules.defines.hidden.Add("_WINDOWS");
		rules.defines.hidden.Add("_USRDLL");
		
		rules.include_dirs.exposed.Add("Exposed");
	}
	
	private void SetupRules()
	{
		//@TODO
	}
	
	private List<IRelativeFilePath> FindFiles(string extension, string file_label)
	{
		List<IRelativeFilePath> files = [];

		foreach (string file_str in Directory.GetFiles(path.Get(), extension, SearchOption.AllDirectories))
		{
			IAbsoluteFilePath file = file_str.ToAbsoluteFilePath();
			IRelativeFilePath rel_file = file.GetRelativePathFrom(path);

			files.Add(rel_file);
		}

		if (files.Count != 0)
		{
			Logger.Get().LogDebug($"{name}: Found {files.Count} {file_label} file(s): {String.Join(", ", files)}");
		}
		else
		{
			Logger.Get().LogDebug($"{name}: No {file_label} files were found");
		}

		return files;
	}
}
