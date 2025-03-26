// CopyRight FlareHorz Team. All Rights Reserved.

using System.Collections.Generic;
using FlareBuildTool.Premake;
using FlareBuildTool.Target;
using FlareCore;
using FlareCore.Logger;

namespace FlareBuildTool.Solution;

/// <summary>
/// A representation of solution we need to parse and build.
/// Contains info about all targets.
/// </summary>
[FlareBuildToolAPI]
public class FSolution : FFlareObject
{
	/// <summary>
	/// Calls when object of that class is created.
	/// </summary>
	protected override void OnObjectCreated()
	{
		base.OnObjectCreated();
		
		LuaFile = CreateObject<FFile>("LuaFile");
		PremakeAPI = CreateObject<FPremakeAPI>("PremakeAPI");
	}

	/// <summary>
	/// Generate lua file for this solution.
	/// </summary>
	public void GenerateLuaFile()
	{
		return; // TODO: REMOVE
		
		// Open lua file
		PremakeAPI.PremakeLuaFile = LuaFile;
		PremakeAPI.PremakeLuaFile.LoadFile(PremakeAPI.PremakeLuaFile.FilePath);
		
		// Write to that file
		GenerateCode(FGlobal.BuildTool.SolutionTargets);
		
		// Close it
		PremakeAPI.PremakeLuaFile.Close();
	}

	/// <summary>
	/// Generate code for lua file.
	/// </summary>
	private void GenerateCode(List<FTarget> Targets)
	{
		PremakeAPI.CopyRight();
		PremakeAPI.Include("FlareBuildTool/FlarePremakeExtension.lua");
		
		GenerateGlobalCode(Targets);
	}

	/// <summary>
	/// Generate global code for solution.
	/// </summary>
	private void GenerateGlobalCode(List<FTarget> Targets)
	{
		// TODO: Startup target
		
		PremakeAPI.SolutionItems(new string[]
		{
			"README.md", ".gitignore"
		});
		
		// TODO: PremakeAPI.BuildRules() for each C# target
		// FGlobal.BuildTool.SolutionTargets[0].TargetRules.TargetLanguage == ETargetLanguage.CS
	}

	/// <summary>
	/// Generate code for one target.
	/// </summary>
	private void GenerateCodeForTarget(FTarget Target)
	{
	}

	/// <summary>
	/// Quote the text.
	/// </summary>
	private string Quote(string InText)
	{
		return "\"" + InText + "\"";
	}
	
	/// <summary>
	/// Lua file of this solution.
	/// </summary>
	public FFile LuaFile;

	/// <summary>
	/// Premake5 API.
	/// </summary>
	private FPremakeAPI PremakeAPI;
}
