﻿// CopyRight FlareHorz Team. All Rights Reserved.

using System.Collections.Generic;
using FlareCore;
using FlareBuildTool.Solution;

namespace FlareBuildTool.Target;

/// <summary>
/// Rules describing target for .Target.cs file.
/// </summary>
[FlareBuildToolAPI]
public class FTargetRules : FFlareObject
{
	/// <summary>
	/// Calls when object of that class is created.
	/// </summary>
	protected override void OnObjectCreated() 
	{
		base.OnObjectCreated();
		
		TargetType = ETargetType.Executable;
		TargetLanguage = ETargetLanguage.CPP;
		CPP_Version = 17;
		CS_Version = 11.0f;
		DotNetFrameworkVersion = 4.8f;
		bRunHeaderTool = false;
		HeaderToolRunCommand = "v_RunHeaderTool";
		PreBuildCommands = new List<string>();
		PostBuildCommands = new List<string>();
		LinkedTargets = new List<string>();
		Links = new List<string>();
		IncludeDirectories = new List<string>();
		LibraryDirectories = new List<string>();
		Files = new List<string>();
		Defines = new List<string>();
		DevelopmentConfigurationRules = new FConfigurationTargetRules();
		DevelopmentConfigurationRules.ForWhichConfiguration = EConfiguration.Development;
		DebugConfigurationRules = new FConfigurationTargetRules();
		DebugConfigurationRules.ForWhichConfiguration = EConfiguration.Debug;
		ShippingConfigurationRules = new FConfigurationTargetRules();
		ShippingConfigurationRules.ForWhichConfiguration = EConfiguration.Shipping;
		bStartupProject = false;
		Group = "";
	}
	
	/// <summary>
	/// Solution items.
	/// </summary>
	public static List<string> SolutionItems = new List<string>();
	
	/// <summary>
	/// Type of the target.
	/// </summary>
	public ETargetType TargetType;

	/// <summary>
	/// Language this target use.
	/// </summary>
	public ETargetLanguage TargetLanguage;

	/// <summary>
	/// C++ dialect. We support 11, 14, 17, 20.
	/// Only used when TargetLanguage is C++.
	/// </summary>
	public int CPP_Version;

	/// <summary>
	/// C# version. We support up to 11.0.
	/// Only used when TargetLanguage is C#.
	/// </summary>
	public float CS_Version;

	/// <summary>
	/// .NET Framework version for C#.
	/// Only used when TargetLanguage is C#.
	/// </summary>
	public float DotNetFrameworkVersion;

	/// <summary>
	/// Command to run Flare Header Tool.
	/// </summary>
	public string HeaderToolRunCommand;
	
	/// <summary>
	/// Run Dark Header Tool for this project or no.
	/// </summary>
	public bool bRunHeaderTool;
    
	/// <summary>
	/// Files. You can use "Folder/**/**.{extension}" to add all files with specific extension.
	/// </summary>
	public List<string> Files;

	/// <summary>
	/// Defines for this target. You can use "MyDefine=SomeCode".
	/// </summary>
	public List<string> Defines;
	
	/// <summary>
	/// Includes that will be used for this target and exposed to others.
	/// </summary>
	public List<string> IncludeDirectories;

	/// <summary>
	/// Libraries this target have that will be exposed to linked targets.
	/// </summary>
	public List<string> LibraryDirectories;
	
	/// <summary>
	/// Rules that are specific for Development configuration.
	/// </summary>
	public FConfigurationTargetRules DevelopmentConfigurationRules;
	
	/// <summary>
	/// Rules that are specific for Debug configuration.
	/// </summary>
	public FConfigurationTargetRules DebugConfigurationRules;
	
	/// <summary>
	/// Rules that are specific for Shipping configuration.
	/// </summary>
	public FConfigurationTargetRules ShippingConfigurationRules;
	
	/// <summary>
	/// Pre-build commands.
	/// </summary>
	public List<string> PreBuildCommands;
	
	/// <summary>
	/// Post-build commands.
	/// </summary>
	public List<string> PostBuildCommands;
	
	/// <summary>
	/// Linked targets. In difference with Links, LinkedTargets will expose public includes and etc to this one.
	/// </summary>
	public List<string> LinkedTargets;

	/// <summary>
	/// Links this target have. Used for third-party generally.
	/// </summary>
	public List<string> Links;
	
	/// <summary>
	/// Is startup project.
	/// </summary>
	public bool bStartupProject;
	
	/// <summary>
	/// Group, actually solution folders. Use dots to make subgroups.
	/// TODO: Subgroups for targets
	/// </summary>
	public string Group;

	/// <summary>
	/// Setup target info.
	/// </summary>
	public void SetTargetInfo(ETargetType InTargetType, ETargetLanguage InTargetLanguage)
	{
		TargetType = InTargetType;
		TargetLanguage = InTargetLanguage;
	}
}
