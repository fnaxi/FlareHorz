@echo off
rem CopyRight FlareHorz Team. All Rights Reserved.

echo Checking that .NET is installed...

for /f "delims=" %%v in ('dotnet --version 2^>nul') do set VERSION=%%v
if defined VERSION (
    echo Found .NET version: %VERSION%
) else (
    goto Error_DotNetIsNotInstalled
)

rem quiet/minimal/normal/detailed/diagnostic
set BUILD_VERBOSITY=detailed

echo Building FlareBuildTool...
dotnet build Source/Tools/FlareBuildTool/FlareBuildTool.csproj -c Debug -a x64 -v %BUILD_VERBOSITY%
if errorlevel 1 goto Error_BuildToolCompileFailed

echo Building BuildRules...
dotnet build Source/Tools/FlareBuildRules/FlareBuildRules.csproj -c Debug -a x64 -v %BUILD_VERBOSITY%
if errorlevel 1 goto Error_RulesCompileFailed

echo Running FlareBuildTool...
call Binaries\FlareBuildTool.exe -ProjectFiles -Log?Debug -NoMutex
if errorlevel 1 goto Error_FlareBuildToolFailure

goto Exit

:Error_DotNetIsNotInstalled
echo.
echo .NET is not installed!
echo.
goto Exit

:Error_BuildToolCompileFailed
echo.
echo FlareBuildTool failed to compile!
echo.
goto Exit

:Error_RulesCompileFailed
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
