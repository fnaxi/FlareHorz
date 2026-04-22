// CopyRight FlareHorz Team. All Rights Reserved.

using FlareBuildTool;
using FlareCore;

namespace BuildRules.ThirdParty;

public class CImGui : CThirdPartyModuleRules
{
	public CImGui()
	{
		// Using Dear ImGui via a shared library is not recommended
		OutputType = EBuildOutputType.StaticLibrary;
		
		FileDirectories.AddRange(new[]
		{
			CPath.FlareCombine(CScript.ProjectPath, "imgui.h"),
			CPath.FlareCombine(CScript.ProjectPath, "imgui_internal.h"),
			CPath.FlareCombine(CScript.ProjectPath, "imconfig.h"),
			CPath.FlareCombine(CScript.ProjectPath, "imstb_rectpack.h"),
			CPath.FlareCombine(CScript.ProjectPath, "imstb_textedit.h"),
			CPath.FlareCombine(CScript.ProjectPath, "imstb_truetype.h"),
			
			CPath.FlareCombine(CScript.ProjectPath, "imgui.cpp"),
			CPath.FlareCombine(CScript.ProjectPath, "imgui_demo.cpp"),
			CPath.FlareCombine(CScript.ProjectPath, "imgui_draw.cpp"),
			CPath.FlareCombine(CScript.ProjectPath, "imgui_tables.cpp"),
			CPath.FlareCombine(CScript.ProjectPath, "imgui_widgets.cpp"),
			
			CPath.FlareCombine(CScript.ProjectPath, "LICENSE.txt")
		});

		// SDL3 + SDLRenderer3 backend
		{
			FileDirectories.Add(CPath.FlareCombine(CScript.ProjectPath, "backends/imgui_impl_sdl3.**"));
			FileDirectories.Add(CPath.FlareCombine(CScript.ProjectPath, "backends/imgui_impl_sdlrenderer3.**"));
			// FileDirectories.Add(CPath.FlareCombine(CScript.ProjectPath, "examples/example_sdl3_sdlrenderer3/main.cpp"));
			
			PublicDependencyModules.Add("SDL");
		}
		
		PublicIncludeDirectories.Add(CPath.ToFlare(CScript.ProjectPath));
		PublicIncludeDirectories.Add(CPath.FlareCombine(CScript.ProjectPath, "backends"));
	}
}
