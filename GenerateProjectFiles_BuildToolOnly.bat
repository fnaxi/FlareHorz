@echo off
rem CopyRight FlareHorz Team. All Rights Reserved.


echo Running GenerateProjectFiles.bat
call GenerateProjectFiles.bat BuildToolOnly
if errorlevel 1 goto Error_ScriptFailure


goto Exit

:Error_ScriptFailure
echo.
echo GenerateProjectFiles.bat exited with code %errorlevel%!
echo.
goto Exit

:Exit
