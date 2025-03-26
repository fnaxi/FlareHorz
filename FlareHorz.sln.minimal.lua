-- CopyRight FlareHorz Team. All Rights Reserved.
-- ==========================================================================
-- Generated code exported from Flare Build Tool.
-- DO NOT modify this manually! Edit the corresponding build files instead!
-- ==========================================================================
include "FlareBuildTool/FlarePremakeExtension.lua"
workspace "FlareHorz"
solution_items
{
	"README.md",
	".gitignore"
}
build_rules
{
	"FlareCore/FlareCore.Target.cs",
	"FlareBuildTool/FlareBuildTool.Target.cs"
}
startproject "FlareBuildTool"
configurations
{
	"Debug",
	"Development"
}
platforms
{
	"Win64"
}
v_OutputDir = "%{cfg.buildcfg}"

group "Programs"
project "FlareCore"
location "FlareCore"
kind "SharedLib"
language "C#"
csversion "11.0"
dotnetframework "4.8"
targetdir("Binaries/" .. v_OutputDir)
objdir("Intermediate/" .. v_OutputDir)
links
{
	"System",
}
files
{
	"%{wks.location}/FlareCore/**.cs"
}
excludes "%{wks.location}/FlareCore/FlareCore.Target.cs"
filter "configurations:Debug"
defines
{
	"FH_DEBUG"
}
runtime "Debug"
symbols "on"
filter "configurations:Development"
defines
{
	"FH_DEVELOPMENT"
}
runtime "Release"
optimize "on"
filter "system:windows"
systemversion "latest"
group ""

group "Programs"
project "FlareBuildTool"
location "FlareBuildTool"
kind "ConsoleApp"
language "C#"
csversion "11.0"
dotnetframework "4.8"
targetdir("Binaries/" .. v_OutputDir)
objdir("Intermediate/" .. v_OutputDir)
links
{
	"System",
	"FlareCore",
}
files
{
	"%{wks.location}/FlareBuildTool/**.cs",
	"%{wks.location}/FlareBuildTool/**.lua"
}
excludes "%{wks.location}/FlareBuildTool/FlareBuildTool.Target.cs"
filter "configurations:Debug"
defines
{
	"FH_DEBUG"
}
runtime "Debug"
symbols "on"
filter "configurations:Development"
defines
{
	"FH_DEVELOPMENT"
}
runtime "Release"
optimize "on"
filter "system:windows"
systemversion "latest"
group ""


