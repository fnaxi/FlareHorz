@echo off
rem CopyRight FlareHorz Team. All Rights Reserved.


rem Download premake and unpack it in Binaries/ directory
set PREMAKE_EXE=premake5.exe
set PREMAKE_ZIP=premake-5.0.0-beta8-windows.zip
set PREMAKE_URL=https://github.com/premake/premake-core/releases/download/v5.0.0-beta8/%PREMAKE_ZIP%

if exist Binaries\%PREMAKE_EXE% (
	echo Premake is already downloaded
	goto PremakeDownloaded
)

echo Downloading premake...
powershell -Command "Invoke-WebRequest -Uri %PREMAKE_URL% -OutFile %PREMAKE_ZIP%"

if not exist Binaries (
	mkdir Binaries
)

echo Extracting %PREMAKE_ZIP% archive...
powershell -Command "Expand-Archive -Path %PREMAKE_ZIP% -DestinationPath Binaries -Force"

del %PREMAKE_ZIP%

:PremakeDownloaded

rem values: quiet/minimal/normal/detailed/diagnostic
set BUILD_VERBOSITY=diagnostic

echo Generating project files for FlareBuildTool...
call Binaries\premake5.exe vs2022 --file=Source/Programs/FlareBuildTool/FlareBuildTool.lua
if errorlevel 1 goto Error_Premake

echo Checking that .NET is installed...

for /f "delims=" %%v in ('dotnet --version 2^>nul') do set VERSION=%%v
if defined VERSION (
    echo Found .NET version: %VERSION%
) else (
    goto Error_DotNetIsNotInstalled
)

echo Building FlareBuildTool...
dotnet build Source/Programs/FlareBuildTool/FlareBuildTool.csproj -c Debug -a x64 -v %BUILD_VERBOSITY%
if errorlevel 1 goto Error_FBTCompileFailed

echo Building BuildRules...
dotnet build Source/BuildRules/BuildRules.csproj -c Debug -a x64 -v %BUILD_VERBOSITY%
if errorlevel 1 goto Error_BuildRulesCompileFailed

if not "%1" == "BuildToolOnly" (
	echo Running FlareBuildTool...
	call Binaries\FlareBuildTool.exe
	if errorlevel 1 goto Error_FlareBuildToolFailure
)

goto Exit

:Error_Premake
echo.
echo Failed to generate project files using premake!
echo.
goto Exit

:Error_DotNetIsNotInstalled
echo.
echo .NET is not installed!
echo.
goto Exit

:Error_FBTCompileFailed
echo.
echo FlareBuildTool failed to compile!
echo.
goto Exit

:Error_BuildRulesCompileFailed
echo.
echo BuildRules failed to compile!
echo.
goto Exit

:Error_FlareBuildToolFailure
echo.
echo FlareBuildTool exited with code %errorlevel%!
echo.
goto Exit

:Exit
pause
