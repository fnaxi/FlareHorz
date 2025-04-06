// CopyRight FlareHorz Team. All Rights Reserved.

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FlareBuildTool.Module;
using FlareCore;
using FlareCore.Logger;
using Microsoft.CSharp;

namespace FlareBuildTool.Target;

/// <summary>
/// A class that describes one project in the solution.
/// </summary>
[FlareBuildToolAPI]
public sealed class FTarget : FFlareObject
{
	/// <summary>
	/// Calls when object of that class is created.
	/// </summary>
	protected override void OnObjectCreated()
	{
		base.OnObjectCreated();
		
		TargetCsFile = CreateObject<FFile>("TargetCsFile");
		TargetRules = CreateObject<FTargetRules>("TargetRules");
		Modules = new List<FModule>();
	}
	
	/// <summary>
	/// Name of this target.
	/// </summary>
	public string Name;

	/// <summary>
	/// .Target.cs file in root of this target's path. Needed to create FTargetRules.
	/// </summary>
	public FFile TargetCsFile;
	
	/// <summary>
	/// Build rules for this target.
	/// </summary>
	public FTargetRules TargetRules;

	/// <summary>
	/// Modules that target have.
	/// </summary>
	public List<FModule> Modules;

	/// <summary>
	/// Compile .Target.cs file at runtime to get FTargetRules.
	/// </summary>
	public void CompileBuildFile()
	{
		FActionTime AT_Compiling = FActionTime.Start("AT_Compiling" + Name, ELogVerbosity.Info);
		
		// Get FTargetRules and save it into our field
		Assembly CompiledAssembly = Assembly.LoadFrom("BuildRules.dll");
		
		Type RuntimeType = CompiledAssembly.GetType("F" + Name + "Target");
		FAssert.Checkf(RuntimeType != null, "Can't find F" + Name + "Target class!");
		FAssert.Checkf(RuntimeType.IsSubclassOf(typeof(FTargetRules)), "F" + Name + "Target should be parent to FTargetRules!");
		
		FTargetRules RuntimeTargetRules = (FTargetRules) Activator.CreateInstance(RuntimeType);
		FAssert.Checkf(RuntimeTargetRules != null, "Failed to cast F" + Name + "Target to FTargetRules");
		
		RuntimeTargetRules.Initialize(Name + "Target");
		
		// Set module rules from .Target.cs
		TargetRules = RuntimeTargetRules;
		
		AT_Compiling.Stop();
	}

	/// <summary>
	/// Handle modules in this target by compiling .Build.cs files and etc.
	/// </summary>
	public void HandleModules()
	{
		if (TargetRules.TargetLanguage != ETargetLanguage.CPP) return;
		
		// Find all modules in the target
		foreach (string ModulePath in Directory.GetDirectories(Path.GetDirectoryName(TargetCsFile.FilePath)))
		{
			string ModuleName = Path.GetFileNameWithoutExtension(ModulePath);
			
			FModule Module = CreateObject<FModule>(ModuleName + "Module");
			Module.Name = ModuleName;

			string BuildCsFileName = ModuleName + ".Build.cs";
			string BuildCsFilePath = Path.Combine(ModulePath, BuildCsFileName);
			FAssert.Checkf(File.Exists(BuildCsFilePath), "There's no " + BuildCsFileName + " file for " + ModuleName + " module!");
			
			Module.ModuleRules = CompileModuleFile(BuildCsFilePath, BuildCsFileName, ModuleName);
			
			Modules.Add(Module);
		}

		if (Modules.Count == 0)
		{
			FLog.Warn("There are no modules in a C++ target!");
		}
	}

	/// <summary>
	/// Print all info about target.
	/// </summary>
	public void PrintTargetInfo()
	{
		FLog.Debug("========== " + Name.ToUpper() + " TARGET INFO ==========");
		FLog.Debug("Target Name: " + Name);
		FLog.Debug("Group: "  + TargetRules.Group);
		FLog.Debug("Language: " + TargetRules.TargetLanguage);
		FLog.Debug("Type: " + TargetRules.TargetType);
		FLog.Debug("IsStartup: " + TargetRules.bStartupTarget);
		if (TargetRules.TargetLanguage == ETargetLanguage.CS)
		{
			FLog.Debug("C# Ver: " + TargetRules.CS_Version);
			FLog.Debug(".NET Ver: " + TargetRules.DotNetFrameworkVersion);
		}
		else if (TargetRules.TargetLanguage == ETargetLanguage.CPP)
		{
			FLog.Debug("C++ Ver: " + TargetRules.CPP_Version);
		}
		FLog.Debug("RunHT: " + TargetRules.bRunHeaderTool);
		if (TargetRules.bRunHeaderTool)
		{
			FLog.Debug("HTCmd: " + TargetRules.HeaderToolRunCommand);
		}
		foreach (string FilePath in TargetRules.Files) { FLog.Debug("FilePaths: " + FilePath); }
		foreach (string Define in TargetRules.Defines) { FLog.Debug("Define: " + Define); }
		foreach (string Link in TargetRules.Links) { FLog.Debug("Link: " + Link); }
		foreach (string LinkedTarget in TargetRules.LinkTargets) { FLog.Debug("LinkedTarget: " + LinkedTarget); }
		foreach (string IncludeDirectory in TargetRules.IncludeDirectories) { FLog.Debug("IncludeDirectory: " + IncludeDirectory); }
		foreach (string LibraryDirectory in TargetRules.LibraryDirectories) { FLog.Debug("LibraryDirectory: " + LibraryDirectory); }
		// TODO: Print modules info
		// TODO: print configuration rules info
		FLog.Debug("==================== END ====================");
	}

	/// <summary>
	/// Compile .Build.cs file for module.
	/// </summary>
	private FModuleRules CompileModuleFile(string InFilePath, string InFileName, string ModuleName)
	{
		FActionTime AT_Compiling = FActionTime.Start("AT_Compiling" + Name + "-" + ModuleName, ELogVerbosity.Info);
		
		// Getting FModuleRules
		Assembly CompiledAssembly = Assembly.LoadFrom("BuildRules.dll");
		
		Type RuntimeType = CompiledAssembly.GetType("F" + Name + ModuleName + "Module");
		FAssert.Checkf(RuntimeType != null, "Can't find F" + Name + ModuleName + "Module class!");
		FAssert.Checkf(RuntimeType.IsSubclassOf(typeof(FModuleRules)), "F" + Name + ModuleName + "Module should be parent to FModuleRules!");
			
		FModuleRules RuntimeModuleRules = (FModuleRules) Activator.CreateInstance(RuntimeType);
		FAssert.Checkf(RuntimeModuleRules != null, "Failed to cast F" + Name + ModuleName + "Module to FModuleRules");
			
		RuntimeModuleRules.Initialize(Name + ModuleName + "Module");
		
		AT_Compiling.Stop();
		
		// Set module rules from .Target.cs
		return RuntimeModuleRules;
	}
}
