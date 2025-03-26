// CopyRight FlareHorz Team. All Rights Reserved.

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using FlareBuildTool.Target.Module;
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
	public void ComplieBuildFile()
	{
		FActionTime AT_Compiling = FActionTime.Start("AT_Compiling");
		
		// Info for compiling
		CSharpCodeProvider CodeProvider = new CSharpCodeProvider();
		CompilerParameters CompilerParams = new CompilerParameters();
		
		// Setup links to have Flare Build Tool API available
		CompilerParams.ReferencedAssemblies.Add(FGlobal.SolutionPath + @"\Binaries\" + FGlobal.Configuration + @"\FlareBuildTool.exe");
		CompilerParams.ReferencedAssemblies.Add(FGlobal.SolutionPath + @"\Binaries\" + FGlobal.Configuration + @"\FlareCore.dll");

		// Compile it
		CompilerResults Result = CodeProvider.CompileAssemblyFromFile(CompilerParams, TargetCsFile.FilePath);
		if (!Result.Errors.HasErrors)
		{
			FLog.Info("Successufuly compiled " + TargetCsFile.FileName);
			
			// Get FTargetRules and save it into our field
			Assembly CompiledAssembly = Result.CompiledAssembly;
			
			Type RuntimeType = CompiledAssembly.GetType("F" + Name + "Target");
			FAssert.Checkf(RuntimeType != null, "Can't find F" + Name + "Target class!");
			FAssert.Checkf(RuntimeType.IsSubclassOf(typeof(FTargetRules)), "F" + Name + "Target should be parent to FTargetRules!");
			
			FTargetRules RuntimeTargetRules = (FTargetRules) Activator.CreateInstance(RuntimeType);
			FAssert.Checkf(RuntimeTargetRules != null, "Failed to cast F" + Name + "Target to FTargetRules");
			
			// Set module rules from .Target.cs
			TargetRules = RuntimeTargetRules;
		}
		else
		{
			FLog.Error("Errors occured while compiling " + TargetCsFile.FileName);
			foreach (CompilerError CompilerCurrentError in Result.Errors)
			{
				FLog.Error("> " + CompilerCurrentError.ErrorText);
			}
		}
		
		AT_Compiling.Stop();
	}

	/// <summary>
	/// Handle modules in this target by compiling .Build.cs files and etc.
	/// </summary>
	public void HandleModules()
	{
		// TODO: Modules
	}
}
