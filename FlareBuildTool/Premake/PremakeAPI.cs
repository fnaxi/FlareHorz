// CopyRight FlareHorz Team. All Rights Reserved.

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
	
	public void StartProject(string Name) { Param("startproject", Name); }
	
	public void GroupStart(string Name) { Param("group", Name); }
	
	public void GroupEnd() { Param("group", ""); }
	
	public void Project(string Name) { Param("project", Name); }
	
	public void Location(string Name) { Param("location", Name); }
	
	public void Type(string Name) { Param("kind", Name); }
	
	public void Language(string Name) { Param("language", Name); }
	
	public void CS_Version(float Version) { Param("csversion", Version.ToString()); }
	
	public void CPP_Version(int Version) { Param("cppdialect", "C++" + Version.ToString()); }

	public void DotNetFrameworkVersion(float Version) { Param("dotnetframework", Version.ToString()); }
	
	public void BinariesPath(string Path) { Constructor("targetdir", Path); }
	
	public void IntermediatePath(string Path) { Constructor("objdir", Path); }
	
	public void Exlude(string Name) { WksParam("excludes", Name); }
	
	public void Filter(string Name) { Param("filter", Name); }
	
	public void FilterConfiguration(string Name) { Filter("configurations:" + Name); }
	
	public void COND_Runtime() { Condition("runtime"); }
	
	public void COND_Symbols() { Condition("symbols"); }
	
	public void COND_Optimize() { Condition("optimize"); }
	
	//////////////////////////////////////////////////////////////
	/// Lists
	//////////////////////////////////////////////////////////////
	
	public void SolutionItems(string[] Items) { List("solution_items", Items); }
	
	public void BuildRules(string[] Items) { List("build_rules", Items); }
	
	public void Configurations(string[] Items) { List("configurations", Items); }
	
	public void Platforms(string[] Items) { List("platforms", Items); }
	
	public void Links(string[] Items) { List("links", Items); }
	
	public void Files(string[] Items) { List("files", Items); }
	
	public void Defines(string[] Items) { List("defines", Items); }
	
	public void IncludeDirectories(string[] Items) { List("includedirs", Items); }
	
	public void LibraryDirectories(string[] Items) { List("libdirs", Items); }
	
	public void PreBuildCommands(string[] Items) { List("prebuildcommands", Items); }
	
	public void PostBuildCommands(string[] Items) { List("postbuildcommands", Items); }
	
	//////////////////////////////////////////////////////////////
	/// Other
	//////////////////////////////////////////////////////////////

	public void Variable(string Name, string Value)
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

	private void WksList(string Name, string[] InItems)
	{
		List(Name, InItems, true);
	}
	
	private void List(string Name, string[] InItems, bool bStartWithWorkspace = false)
	{
		PremakeLuaFile.WriteSrc(Name + " {");
		foreach (string Item in InItems)
		{
			PremakeLuaFile.WriteSrc("\"" + (bStartWithWorkspace ? "%{wks.location}" : "") + Item + "\",");
		}
		PremakeLuaFile.WriteSrc("}");
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
