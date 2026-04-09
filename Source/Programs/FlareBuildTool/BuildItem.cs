// CopyRight FlareHorz Team. All Rights Reserved.

global using static FlareBuildTool.CGlobalRules;

using System;
using System.Collections.Generic;
using System.Reflection;
using FlareCore;

namespace FlareBuildTool;

public static class CGlobalRules
{
	/** C++ dialect to use, supported ones are 11, 14, 17 and 20. */
	public static Int32 CxxVersion = 17;

	/** C# version to use. */
	public static float CSharpVersion = 11.0f;

	/** .NET Framework version to use for C#. */
	public static float DotNetFrameworkVersion = 4.8f;
}

public class CBuildItem // DO NOT USE DIRECTLY!
{
	public CBuildItem(string InScriptFilePath)
	{
		ScriptFilePath = InScriptFilePath;
		Name = CPath.GetFileNameWithoutDoubleExtension(ScriptFilePath);
	}
	
	/** Name of the target. Set basing on name of <c>.Build.cs</c> file. */
	public string Name;
	public string ScriptFilePath;
	
	public virtual string GetRootPath()
	{
		return CheckNoEntry("GetRootPath() must be overriden in child classes of CBuildItem!").ToString();
	}
	
	public virtual void GeneratePremakeCode(CPremakeFileHandle Premake) { }

	public virtual void SetupRules(in List<CBuildItem> Others) { }
	public virtual void DefaultSetupRules() { }
	
	protected T GatherRules<T>(string TypeName)
	{
		Assembly BuildRules = Assembly.LoadFrom( CPath.Combine(BinariesPath, "BuildRules.dll") );

		Type RuntimeType = BuildRules.GetType(TypeName);
		Verify(RuntimeType != null, $"Can't find {TypeName} class!");
		Verify(RuntimeType.IsSubclassOf(typeof(T)), $"{TypeName} should inherit {typeof(T)}!");
		
		T RuntimeRules = (T) Activator.CreateInstance(RuntimeType);
		Verify(RuntimeRules != null, $"Failed to cast RuntimeType to {typeof(T)}!");
		
		return RuntimeRules;
	}
}
