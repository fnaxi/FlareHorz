// CopyRight © FlareHorz Team. All Rights Reserved.

using System.Diagnostics;
using System.Xml.Linq;
using FlareBuildTool.Configuration;
using FlareCore;
using NDepend.Path;

namespace FlareBuildTool.ProjectFiles;

public class ProjectFileGenerator
{
	public void Create(IAbsoluteDirectoryPath path, FlareModule mod)
	{
		string file_name = $"{mod.name}.vcxproj";
		IAbsoluteFilePath file_path = path.GetChildFileWithName(file_name);

		XDocument vcxproj = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), GenerateVcxprojFile(mod));
		{
			vcxproj.Save(file_path.Get());
		}

		Logger.Get().LogInformation($"Created {file_path.GetRelativePathFrom(Global.engine.root_path)} project file");
	}

	private static XElement GenerateVcxprojFile(FlareModule mod)
	{
		XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";
		
		XElement root_element = new XElement(
			"Project",
			new XAttribute("DefaultTargets", "Build")/*,
			new XAttribute(XNamespace.Xmlns + "ms", ns.NamespaceName)*/);
		
		XElement configurations = new XElement(
			"ItemGroup",
			new XAttribute("Label", "ProjectConfigurations"));
		{
			foreach (string conf in FlareBuildTool.configurations)
			{
				foreach (string platform in FlareBuildTool.platforms)
				{
					configurations.Add(CreateConfiguration(conf, platform));
				}
			}

			root_element.Add(configurations);
		}
		
		root_element.Add(new XElement(
			"PropertyGroup",
			new XElement("OutDir", "$(SolutionDir)Binaries\\"),
			new XElement("IntDir", "$(SolutionDir)Intermediate\\")));
		
		root_element.Add(new XElement(
			"PropertyGroup", 
			new XAttribute("Label", "Globals"),
			new XElement("VCProjectVersion", "15.0"),
			new XElement("ProjectGuid", "{" + mod.uid.ToString().ToUpper() + "}"),
			new XElement("Keyword", "Win32Proj"),
			new XElement("RootNamespace", mod.name),
			new XElement("WindowsTargetPlatformVersion", "10.0")));

		root_element.Add(new XElement(
			"Import",
			new XAttribute("Project", "$(VCTargetsPath)\\Microsoft.Cpp.Default.props")));
		
		root_element.Add(new XElement(
			"PropertyGroup",
			new XElement("PreferredToolArchitecture", "x64")));

		//@TODO: Revisit this
		foreach (string conf in FlareBuildTool.configurations)
		{
			foreach (string platform in FlareBuildTool.platforms)
			{
				root_element.Add(CreateConfigPropertyGroup(conf, platform, "DynamicLibrary"));
			}
		}
		
		root_element.Add(new XElement(
			"Import",
			new XAttribute("Project", "$(VCTargetsPath)\\Microsoft.Cpp.props")));
		
		root_element.Add(new XElement(
			"ImportGroup",
			new XAttribute("Label", "ExtensionSettings")));
		
		root_element.Add(new XElement(
			"ImportGroup",
			new XAttribute("Label", "Shared")));
		
		foreach (string conf in FlareBuildTool.configurations)
		{
			foreach (string platform in FlareBuildTool.platforms)
			{
				root_element.Add( CreatePropertySheet(conf, platform) );
			}
		}
		
		root_element.Add(new XElement(
			"PropertyGroup",
			new XAttribute("Label", "UserMacros")));
		
		foreach (string conf in FlareBuildTool.configurations)
		{
			foreach (string platform in FlareBuildTool.platforms)
			{
				root_element.Add(new XElement(
					"PropertyGroup",
					new XAttribute("Condition", $"'$(Configuration)|$(Platform)'=='{conf}|{platform}'"),
					new XElement("LinkIncremental", IsDebugStr(conf))));
			}
		}
		foreach (string conf in FlareBuildTool.configurations)
		{
			foreach (string platform in FlareBuildTool.platforms)
			{
				root_element.Add(CreateItemDefinitionGroup(conf, platform, mod));
			}
		}

		XElement cl_compile = new XElement("ItemGroup");
		{
			foreach (IRelativeFilePath source in mod.source_files)
			{
				cl_compile.Add(new XElement("ClCompile", new XAttribute("Include", source.Get())));
			}
			
			root_element.Add(cl_compile);
		}
		
		XElement cl_include = new XElement("ItemGroup");
		{
			foreach (IRelativeFilePath header in mod.header_files)
			{
				cl_include.Add(new XElement("ClInclude", new XAttribute("Include", header.Get())));
			}
			
			root_element.Add(cl_include);
		}
		
		root_element.Add(new XElement(
			"Import",
			new XAttribute("Project", "$(VCTargetsPath)\\Microsoft.Cpp.targets")));

		root_element.Add(new XElement(
			"ImportGroup",
			new XAttribute("Label", "ExtensionTargets")));
		
		return root_element;
	}

	private static XElement CreateConfiguration(string conf, string platform)
	{
		return new XElement(
			"ProjectConfiguration",
			new XAttribute("Include", $"{conf}|{platform}"),
			new XElement("Configuration", conf),
			new XElement("Platform", platform)
		);
	}

	private static XElement CreateConfigPropertyGroup(string conf, string platform, string type)
	{
		XElement group = new XElement(
			"PropertyGroup",
			new XAttribute("Condition", $"'$(Configuration)|$(Platform)'=='{conf}|{platform}'"),
			new XAttribute("Label", "Configuration"),
			new XElement("ConfigurationType", type),
			new XElement("UseDebugLibraries", IsDebugStr(conf)),
			new XElement("PlatformToolset", "$(DefaultPlatformToolset)"),
			new XElement("CharacterSet", "Unicode")
		);

		if( !IsDebug(conf) )
		{
			group.Add(new XElement("WholeProgramOptimization", "true"));
		}
		
		return group;
	}
	
	private static XElement CreatePropertySheet(string conf, string platform)
	{
		return new XElement(
			"ImportGroup",
			new XAttribute("Label", "PropertySheets"),
			new XAttribute("Condition", $"'$(Configuration)|$(Platform)'=='{conf}|{platform}'"),
			new XElement(
				"Import",
				new XAttribute("Project", "$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props"),
				new XAttribute("Condition", "exists('$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props')"),
				new XAttribute("Label", "LocalAppDataPlatform")
			)
		);
	}

	private static XElement CreateItemDefinitionGroup(string conf, string platform, FlareModule mod)
	{
		return new XElement(
			"ItemDefinitionGroup",
			new XAttribute("Condition", $"'$(Configuration)|$(Platform)'=='{conf}|{platform}'"),
			new XElement(
				"ClCompile",
				new XElement("PrecompiledHeader", "NotUsing"),
				new XElement("WarningLevel", "Level3"),
				new XElement("Optimization", IsDebug(conf) ? "Disabled" : "MaxSpeed"),
				new XElement("SDLCheck", "true"),
				new XElement("PreprocessorDefinitions", $"FH_{conf.ToUpper()};{CreateSeperatedText(mod.rules.defines.Get())}%(PreprocessorDefinitions)"),
				new XElement("ConformanceMode", "true"),
				new XElement("PrecompiledHeaderFile", "pch.h"),
				new XElement("LanguageStandard", $"stdcpp{FlareModule.cpp_standard}"),
				new XElement("AdditionalIncludeDirectories", CreateSeperatedText(mod.rules.include_dirs.Get().Select(x => "$(ProjectDir)" + x).ToList()))
			),
			new XElement(
				"Link",
				new XElement("SubSystem", "Windows"),
				new XElement("GenerateDebugInformation", IsDebugStr(conf)),
				new XElement("AdditionalLibraryDirectories", CreateSeperatedText(mod.rules.lib_dirs.Get())),
				new XElement("AdditionalDependencies", CreateSeperatedText(mod.rules.dependencies.Get()))
			)
		);
	}

	private static string CreateSeperatedText(List<string> items)
		=> items.Aggregate(String.Empty, (x, item) => x + $"{item};");

	private static bool IsDebug(string conf)
		=> conf is "Debug" or "DebugGame";
	
	private static string IsDebugStr(string conf)
		=> IsDebug(conf) ? "true" : "false";
}
