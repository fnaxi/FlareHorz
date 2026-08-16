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
		string file_path = path.GetChildFileWithName(file_name).Get();

		XDocument vcxproj = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), GenerateVcxprojFile(mod));
		{
			vcxproj.Save(file_path);
		}

		Logger.Get().LogInformation($"Created {file_path} project file");
	}

	private static XElement GenerateVcxprojFile(FlareModule mod)
	{
		// XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";
		
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
			"Import",
			new XAttribute("Project", "$(VCTargetsPath)\\Microsoft.Cpp.props")));
		
		root_element.Add(new XElement(
			"Import",
			new XAttribute("Project", "$(VCTargetsPath)\\Microsoft.Cpp.targets")));
		
		root_element.Add(new XElement(
			"ImportGroup",
			new XAttribute("Label", "ExtensionSettings")));
		
		root_element.Add(new XElement(
			"ImportGroup",
			new XAttribute("Label", "ExtensionTargets")));
		
		root_element.Add(new XElement(
			"ImportGroup",
			new XAttribute("Label", "Shared")));
		
		root_element.Add(new XElement(
			"PropertyGroup",
			new XAttribute("Label", "UserMacros")));
		
		root_element.Add(new XElement(
			"PropertyGroup",
			new XElement("PreferredToolArchitecture", "x64")));

		// todo: Revisit this
		foreach (string conf in FlareBuildTool.configurations)
		{
			foreach (string platform in FlareBuildTool.platforms)
			{
				root_element.Add(CreateConfigPropertyGroup(conf, platform, "DynamicLibrary"));
			}
		}
		foreach (string conf in FlareBuildTool.configurations)
		{
			foreach (string platform in FlareBuildTool.platforms)
			{
				root_element.Add( CreatePropertySheet(conf, platform) );
			}
		}
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
				root_element.Add(CreateItemDefinitionGroup(conf, platform, mod.name, mod.rules.hidden_defines));
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
			new XElement("PlatformToolset", "v143"),
			new XElement("CharacterSet", "Unicode")
		);

		if( !IsDebug(conf) )
		{
			group.Add(new XElement("WholeProgramOptimization", "true"));
		}
		
		return group;
	}
	
	private static XElement CreatePropertySheet(string config, string platform)
	{
		return new XElement(
			"ImportGroup",
			new XAttribute("Label", "PropertySheets"),
			new XAttribute("Condition", $"'$(Configuration)|$(Platform)'=='{config}|{platform}'"),
			new XElement(
				"Import",
				new XAttribute("Project", "$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props"),
				new XAttribute("Condition", "exists('$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props')"),
				new XAttribute("Label", "LocalAppDataPlatform")
			)
		);
	}

	private static XElement CreateItemDefinitionGroup(string conf, string platform, string project_name, List<string> defines)
	{
		defines.Add($"FH_{conf.ToUpper()}");
		
		return new XElement(
			"ItemDefinitionGroup",
			new XAttribute("Condition", $"'$(Configuration)|$(Platform)'=='{conf}|{platform}'"),
			new XElement(
				"ClCompile",
				new XElement("PrecompiledHeader", "NotUsing"),
				new XElement("WarningLevel", "Level3"),
				new XElement("Optimization", IsDebug(conf) ? "Disabled" : "MaxSpeed"),
				new XElement("SDLCheck", "true"),
				new XElement("PreprocessorDefinitions", $"{CreatePreprocessorDefinesText(defines)}%(PreprocessorDefinitions)"),
				new XElement("ConformanceMode", "true"),
				new XElement("PrecompiledHeaderFile", "pch.h")
			),
			new XElement(
				"Link",
				new XElement("SubSystem", "Windows"),
				new XElement("GenerateDebugInformation", IsDebugStr(conf))
			)
		);
	}

	private static string CreatePreprocessorDefinesText(List<string> defines)
	{
		return defines.Aggregate(String.Empty, (x, define) => x + $"{define};");
	}

	private static bool IsDebug(string conf)
		=> conf is "Debug" or "DebugGame";
	
	private static string IsDebugStr(string conf)
		=> IsDebug(conf) ? "true" : "false";
}
