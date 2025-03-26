@echo off

echo CopyRight FlareHorz Team. All Rights Reserved.
echo ----------------------------------------------
echo.

echo Generating solution files with FlareHorz.sln.lua
call FlareBuildTool\ThirdParty\Premake\premake5.exe vs2022 --file=FlareHorz.sln.lua
echo Exited with code 0

pause