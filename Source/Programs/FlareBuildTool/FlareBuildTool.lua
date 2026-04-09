-- CopyRight FlareHorz Team. All Rights Reserved.

-- !!! WARNING !!!
-- By making changes to this file you must make sure that these files will behave the same: 
-- BuildRules/Programs/FlareBuildTool.Build.cs, BuildRules/Programs/FlareCore.Build.cs and BuildRules/BuildRules.Build.cs

include ("PremakeExtension.lua")

workspace "FlareBuildTool"

	solution_items
	{
		"FlareBuildTool.lua",
		"PremakeExtension.lua",
		"../../../GenerateProjectFiles.bat",
		"../../../GenerateProjectFiles_BuildToolOnly.bat"
	}

	startproject "FlareBuildTool"

	configurations "Debug"
	platforms "x64"
	architecture "x64"

project "FlareCore"

	location "../FlareCore"
	
	kind "SharedLib"
	language "C#"
	csversion "11"
	dotnetframework "4.8"
	
	targetdir ("../../../Binaries")
	objdir ("../../../Intermediate")
	
	links "System"

	files "%{prj.location}/**.cs"

	runtime "Debug"
	symbols "on"

	defines "DEBUG"
	
	filter "system:windows"
		systemversion "latest"
	--filter end

project "FlareBuildTool"

	location ""
	
	kind "ConsoleApp"
	language "C#"
	csversion "11"
	dotnetframework "4.8"
	
	icon "../../../Images/Logo.ico"

	targetdir ("../../../Binaries")
	objdir ("../../../Intermediate")
	
	links
	{
		"System",
		"FlareCore"
	}

	files "%{prj.location}/**.cs"
	
	runtime "Debug"
	symbols "on"

	defines "DEBUG"
	
	filter "system:windows"
		systemversion "latest"
	--filter end


project "BuildRules"

	location "../../BuildRules"
	
	kind "SharedLib"
	language "C#"
	csversion "11"
	dotnetframework "4.8"
	
	targetdir ("../../../Binaries")
	objdir ("../../../Intermediate")
	
	links
	{
		"System",
		"FlareCore",
		"FlareBuildTool"
	}
	
	files "%{prj.location}/**.cs"
	
	runtime "Debug"
	symbols "on"
	
	defines "DEBUG"
	
	filter "system:windows"
		systemversion "latest"
	--filter end
