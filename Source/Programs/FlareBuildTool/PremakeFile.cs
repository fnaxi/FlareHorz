// CopyRight FlareHorz Team. All Rights Reserved.

using System;
using System.Collections.Generic;
using FlareCore;

namespace FlareBuildTool;

/**
 * Specifies the programming language.
 */
public enum ETargetLanguage
{
	/** Target is implemented in C++. */
	Cxx,
	
	/** Target is implemented in C#. */
	CSharp
}

public enum ERuntimeType
{
	Debug,
	Release
}

public class CProjectInfo
{
	public string Name;
	public string OutputName;
	public string Location;
	public string BinariesPath;
	public string IntermediatePath;
	public EBuildOutputType OutputType;
	public ETargetLanguage Language;
	public List<string> FileDirectories = new();
	public List<string> ExcludeFileDirectories = new();
	public List<string> Links = new();
	public List<string> Defines = new();
	public List<string> IncludeDirectories = new();
	public List<string> LibraryDirectories = new();
	public List<string> PreBuildCommands = new();
	public List<string> PostBuildCommands = new();
	
	/** Allows to write custom premake code for the project. No premake filter is applied. */
	public Action CustomCode = () => {};
}

/**
 * Provides additional utility methods for writing premake scripts.
 * <remarks> There's an exception from the code style rules: private methods are prefixed by _ for helping in maintenance. </remarks>
 */
public class CPremakeFileHandle : CFileHandle
{
	/////////////////////////////////////////////////////////////////
	// PUBLIC API
	/////////////////////////////////////////////////////////////////
	
	public void WriteCopyright()
	{
		_WriteComment(CopyRightNotice);
		_WriteComment("==========================================================================");
		_WriteComment("Generated code exported from Flare Build Tool.");
		_WriteComment("DO NOT modify this manually! Edit the corresponding build files instead!");
		_WriteComment("==========================================================================");
		WriteEmptyLine();
	}
	
	public void WriteKind(EBuildOutputType Kind)
	{
		string KindText = string.Empty;
		switch (Kind)
		{
			case EBuildOutputType.Executable:
				KindText = "ConsoleApp";
				break;
			case EBuildOutputType.DynamicLibrary:
				KindText = "SharedLib";
				break;
			case EBuildOutputType.StaticLibrary:
				KindText = "StaticLib";
				break;
			default:
				CheckNoEntry("Forgot to set OutputType in '.Build.cs' file!");
				break;
		}
		_WriteParam("kind", KindText);
	}

	public void WriteLanguage(ETargetLanguage Language)
	{
		string LanguageText = string.Empty;
		switch (Language)
		{
			case ETargetLanguage.CSharp:
				LanguageText = "C#";
				break;
			case ETargetLanguage.Cxx:
				LanguageText = "C++";
				break;
			default:
				CheckNoEntry();
				break;
		}
		_WriteParam("language", LanguageText);
	}
	
	public void WriteProjectCode(CProjectInfo ProjectInfo)
	{
		WriteProject(ProjectInfo.Name);
		WriteTargetName(ProjectInfo.OutputName);

		WriteLocation(ProjectInfo.Location);

		WriteKind(ProjectInfo.OutputType);
		WriteLanguage(ProjectInfo.Language);
		switch (ProjectInfo.Language)
		{
			case ETargetLanguage.Cxx:
				WriteCxxVersion(CxxVersion);
				break;
			case ETargetLanguage.CSharp:
				WriteCSharpVersion(CSharpVersion);
				WriteDotNetFrameworkVersion(DotNetFrameworkVersion);
				break;
			default:
				CheckNoEntry();
				break;
		}

		WriteBinariesPath(ProjectInfo.BinariesPath);
		WriteIntermediatePath(ProjectInfo.IntermediatePath);
		
		WriteFiles(ProjectInfo.FileDirectories);
		WriteExcludes(ProjectInfo.ExcludeFileDirectories);
		
		WriteLinks(ProjectInfo.Links);

		WriteDefines(ProjectInfo.Defines);

		WriteIncludeDirectories(ProjectInfo.IncludeDirectories);
		WriteLibraryDirectories(ProjectInfo.LibraryDirectories);
		
		WritePreBuildCommands(ProjectInfo.PreBuildCommands);
		WritePostBuildCommands(ProjectInfo.PostBuildCommands);

		ProjectInfo.CustomCode();
		
		WriteConfigurationFilter("Debug");
		WriteConfigurationFilter("Development");
		WriteConfigurationFilter("Shipping");
		
		WriteFilter("system", "windows");
		{
			WriteSystemVersion("latest");
		}
		
		WriteFilterEnd();
	}
	
	/**
	 * <remarks> Requires: vstudio </remarks>
	 */
	public void WriteRegisterSolutionFolder(string Name)
	{
		_WriteList("premake.api.register", new()
		{
			$"name = \"{Name}\"",
			"scope = \"workspace\"",
			"kind = \"list:string\""
		}, false);
		Write("premake.override(premake.vstudio.sln2005, \"projects\", function(base, wks)");
		Write($"if wks.{Name} and #wks.{Name} > 0 then");
		Write("local solution_folder_GUID = \"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\"");
		Write($"premake.push(\"Project(\\\"\" .. solution_folder_GUID .. \"\\\") = \\\"{Name}\\\", \\\"{Name}\\\", \\\"{{\" .. os.uuid(\"{Name}:\" .. wks.name) .. \"}}\\\"\")");
		Write("premake.push(\"ProjectSection(SolutionItems) = preProject\")");
		Write($"for _, path in ipairs(wks.{Name}) do");
		Write("premake.w(path .. \" = \" .. path)");
		Write("end");
		Write("premake.pop(\"EndProjectSection\")");
		Write("premake.pop(\"EndProject\")");
		Write("end");
		Write("base(wks)");
		Write("end)");
	}

	public void WriteConfigMap(Dictionary<string, string> Map)
	{
		List<string> Items = new();
		foreach (KeyValuePair<string, string> Item in Map)
		{
			Items.Add($"[\"{Item.Key}\"] = \"{Item.Value}\"");
		}
		_WriteList("configmap", Items, false);
	}

	public void WriteSolutionFolder(string Name, List<string> Items) { _WriteList(Name, Items); }
	public void WriteRequire(string ModName) { _WriteConstructor("require", $"'{ModName}'"); }
	public void WriteWorkspace(string Name) { _WriteParam("workspace", Name); }
	public void WriteStartProject(string Name) { _WriteParam("startproject", Name); }
	public void WriteToolset(string Name) { _WriteParam("toolset", Name); }
	public void WriteConfigurations(List<string> Configurations) { _WriteList("configurations", Configurations); }
	public void WritePlatforms(List<string> Platforms) { _WriteList("platforms", Platforms); }
	public void WriteRemovePlatforms(List<string> Platforms) { _WriteList("removeplatforms", Platforms); }
	public void WriteFlags(List<string> Flags) { _WriteList("flags", Flags); }
	public void WriteArchitecture(string Architectures) { _WriteParam("architecture", Architectures); }
	public void WriteGroupBegin(string Name) { _WriteParam("group", Name); }
	public void WriteGroupEnd() { _WriteParam("group", ""); }
	public void WriteProject(string Name) { _WriteParam("project", Name); }
	public void WriteLocation(string Name) { _WriteParam("location", Name); }
	public void WriteCxxVersion(Int32 Version) { _WriteParam("cppdialect", $"C++{Version.ToString()}"); }
	public void WriteCSharpVersion(float Version) { _WriteParam("csversion", $"{Version.ToString()}"); }
	public void WriteDotNetFrameworkVersion(float Version) { _WriteParam("dotnetframework", Version.ToString()); }
	public void WriteBinariesPath(string Path) { _WriteConstructor("targetdir", $"\"{Path}\""); }
	public void WriteIntermediatePath(string Path) { _WriteConstructor("objdir",  $"\"{Path}\""); }
	public void WriteFiles(List<string> Files) { _WriteList("files", Files); }
	public void WriteLinks(List<string> Links) { _WriteList("links", Links); }
	
	public void WriteConfigurationFilter(string Configuration) { WriteFilter("configurations", Configuration); }
	
	public void WritePlatformFilter(string Platform) { WriteFilter("platforms", Platform); }
	public void WriteFilter(string Collection, string Filter) { _WriteParam("filter", $"{Collection}:{Filter}"); }
	public void WriteFilterEnd() { Write("filter {}"); }
	public void WriteDefines(List<string> Defines) { _WriteList("defines", Defines); }
	public void WriteVariable(string Var, string Value) { Write($"{Var} = {Value}"); }
	public void WriteRuntime(ERuntimeType Type) { _WriteParam("runtime", Type.ToString()); }
	public void WriteSymbolsOn() { _WriteCondition("symbols"); }
	public void WriteOptimizeOn() { _WriteCondition("optimize"); }
	public void WriteSystemVersion(string Version) { _WriteParam("systemversion", Version); }
	public void WriteIncludeDirectories(List<string> Directories) { _WriteList("includedirs", Directories); }
	public void WriteLibraryDirectories(List<string> Directories) { _WriteList("libdirs", Directories); }
	public void WritePreBuildCommands(List<string> Commands) { _WriteList("prebuildcommands", Commands); }
	public void WritePostBuildCommands(List<string> Commands) { _WriteList("postbuildcommands", Commands); }
	public void WriteExcludes(List<string> Files) { _WriteList("excludes", Files); }
	public void WriteTargetName(string Name) { _WriteParam("targetname", Name); }
	
	/////////////////////////////////////////////////////////////////
	// PRIVATE API
	/////////////////////////////////////////////////////////////////
	
	private void _WriteParam(string Name, string Value)
	{
		Write($"{Name} \"{Value}\"");
	}
	
	private void _WriteList(string Var, List<string> Values, bool bStringValues = true)
	{
		Write(Var);
		Write("{");
		foreach (string Value in Values)
		{
			Write(bStringValues ? $"\t\"{Value}\"," : $"\t{Value},");
		}
		Write("}");
	}
	
	private void _WriteCondition(string Name)
	{
		Write($"{Name} \"on\"");
	}
	
	private void _WriteConstructor(string Name, string Value)
	{
		Write($"{Name} ({Value})");
	}
	
	private void _WriteComment(string Text)
	{
		Write($"-- {Text}");
	}
}
