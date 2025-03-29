// CopyRight FlareHorz Team. All Rights Reserved.

using System.Collections.Generic;
using FlareBuildTool.Solution;
using FlareBuildTool.Target;
using FlareCore;

namespace FlareBuildTool.Premake;

/// <summary>
/// API for interaction with Premake5.
/// </summary>
[FlareBuildToolAPI]
public class FPremakeAPI : FFlareObject
{
	/// <summary>
	/// Calls when object of that class is created.
	/// </summary>
	protected override void OnObjectCreated()
	{
		base.OnObjectCreated();
	}

	//////////////////////////////////////////////////////////////
	/// Params
	//////////////////////////////////////////////////////////////
	
	public void Include(string LuaFile) { Param("include", LuaFile); }

	public void Workspace(string Name) { Param("workspace", Name); }
	
	public void StartupProject(string Name) { Param("startproject", Name); }
	
	public void GroupStart(string Name) { Param("group", Name); }
	
	public void GroupEnd() { Param("group", ""); }
	
	public void Project(string Name) { Param("project", Name); }
	
	public void Location(string Name) { Param("location", Name); }

	public void TargetType(ETargetType TargetType)
	{
		string StrType = "UNKNOWN_TYPE";
		switch (TargetType)
		{
			case ETargetType.Executable:
				StrType = "ConsoleApp";
				break;
			case ETargetType.DynamicLibrary:
				StrType = "SharedLib";
				break;
			case ETargetType.StaticLibrary:
				StrType = "StaticLib";
				break;
		}
		Param("kind", StrType);
	}

	public void Language(ETargetLanguage Language)
	{
		string StrLanguage = "UNKNOWN";
		switch (Language)
		{
			case ETargetLanguage.CPP:
				StrLanguage = "C++";
				break;
			case ETargetLanguage.CS:
				StrLanguage = "C#";
				break;
			default:
				FAssert.CheckNoEntry();
				break;
		}
		Param("language", StrLanguage);
	}
	
	public void CS_Version(float Version) { Param("csversion", Version.ToString()); }
	
	public void CPP_Version(int Version) { Param("cppdialect", "C++" + Version.ToString()); }

	public void DotNetFrameworkVersion(float Version) { Param("dotnetframework", Version.ToString()); }
	
	public void BinariesPath(string Path) { Constructor("targetdir", Path); }
	
	public void IntermediatePath(string Path) { Constructor("objdir", Path); }
	
	public void Exlude(string Name) { WksParam("excludes", Name); }
	
	public void Filter(string Name) { Param("filter", Name); }

	public void FilterConfiguration(EConfiguration Conf)
	{
		Filter("configurations:" + Conf.ToString());
	}

	public void Runtime(EConfiguration Conf)
	{
		string ParamName = "runtime";
		switch (Conf)
		{
			case EConfiguration.Debug:
				ParamName = "Debug";
				break;
			case EConfiguration.Development:
				FAssert.CheckNoEntry();
				break;
			case EConfiguration.Shipping:
				ParamName = "Release";
				break;
		}
		Param("runtime", ParamName);
	}
	
	public void COND_Symbols() { Condition("symbols"); }
	
	public void COND_Optimize() { Condition("optimize"); }
	
	//////////////////////////////////////////////////////////////
	/// Lists
	//////////////////////////////////////////////////////////////
	
	public void SolutionItems(string[] Items) { List("solution_items", Items); }
	public void SolutionItems(List<string> Items) { List("solution_items", Items); }
	
	public void BuildRules(string[] Items) { List("build_rules", Items); }
	public void BuildRules(List<string> Items) { List("build_rules", Items); }
	
	public void Configurations(string[] Items) { List("configurations", Items); }
	
	public void Platforms(string[] Items) { List("platforms", Items); }
	public void Platforms(List<string> Items) { List("platforms", Items); }
	
	public void Links(string[] Items) { List("links", Items); }
	public void Links(List<string> Items) { List("links", Items); }
	
	public void Files(string[] Items) { WksList("files", Items); }
	public void Files(List<string> Items) { WksList("files", Items); }
	
	public void Defines(string[] Items) { List("defines", Items); }
	public void Defines(List<string> Items) { List("defines", Items); }
	
	public void IncludeDirectories(string[] Items) { WksList("includedirs", Items); }
	public void IncludeDirectories(List<string> Items) { WksList("includedirs", Items); }
	
	public void LibraryDirectories(string[] Items) { WksList("libdirs", Items); }
	public void LibraryDirectories(List<string> Items) { WksList("libdirs", Items); }
	
	public void PreBuildCommands(string[] Items) { List("prebuildcommands", Items); }
	public void PreBuildCommands(List<string> Items) { List("prebuildcommands", Items); }
	
	public void PostBuildCommands(string[] Items) { List("postbuildcommands", Items); }
	public void PostBuildCommands(List<string> Items) { List("postbuildcommands", Items); }
	
	//////////////////////////////////////////////////////////////
	/// Other
	//////////////////////////////////////////////////////////////

	public void EmptyLine()
	{
		PremakeLuaFile.WriteSrc("");
	}
	
	public void StrVariable(string Name, string Value)
	{
		PremakeLuaFile.WriteSrc(Name + "=\""  + Value + "\"");
	}

	public void CopyRight()
	{
		Comment("CopyRight FlareHorz Team. All Rights Reserved.");
		Comment("==========================================================================");
		Comment("Generated code exported from Flare Build Tool.");
		Comment("DO NOT modify this manually! Edit the corresponding build files instead!");
		Comment("==========================================================================");
	}
	
	public void Comment(string InComment)
	{
		PremakeLuaFile.WriteSrc("-- " + InComment);
	}
	
	public void SystemVersion()
	{
		Filter("system:windows");
		Param("systemversion", "latest");
	}
	
	//////////////////////////////////////////////////////////////
	/// Internal
	//////////////////////////////////////////////////////////////

	private void WksList(string Name, string[] InItems) { List(Name, InItems, true); }
	private void WksList(string Name, List<string> InItems) { List(Name, InItems, true); }
	
	private void List(string Name, string[] InItems, bool bStartWithWorkspace = false)
	{
		PremakeLuaFile.WriteSrc(Name);
		PremakeLuaFile.WriteSrc("{ -- BEGIN");
		foreach (string Item in InItems)
		{
			PremakeLuaFile.WriteSrc("\t\"" + (bStartWithWorkspace ? "%{wks.location}" : "") + Item + "\",");
		}
		PremakeLuaFile.WriteSrc("} -- END");
	}
	
	private void List(string Name, List<string> InItems, bool bStartWithWorkspace = false)
	{
		List(Name, InItems.ToArray(), bStartWithWorkspace);
	}

	private void Param(string Name, string Value)
	{
		PremakeLuaFile.WriteSrc(Name + " \""  + Value + "\"");
	}

	private void Condition(string Name)
	{
		PremakeLuaFile.WriteSrc(Name + " \"on\"");
	}
	
	private void WksParam(string Name, string Value)
	{
		PremakeLuaFile.WriteSrc(Name + " \"%{wks.location}"  + Value + "\"");
	}
	
	private void Constructor(string Name, string Value)
	{
		PremakeLuaFile.WriteSrc(Name + "("  + Value + ")");
	}

	/// <summary>
	/// Premake5 lua file.
	/// </summary>
	public FFile PremakeLuaFile;
}
