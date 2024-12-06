@echo off
:: backup current directory
pushd %CD%

:: set batch file directory as current
cd /d %~dp0%

start /b /wait tp/Sharpmake/Sharpmake.Application.exe /sources('Main.Sharpmake.cs') /generateDebugSolution /debugsolutionpath('debug/')

IF %ERRORLEVEL% neq 0 GOTO ERROR

PAUSE

GOTO END

:ERROR
echo !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
echo There has been some project generation errors.
echo !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

PAUSE

:END

exit /b %ERRORLEVEL%

