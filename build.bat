@echo off
set PACKAGE_NAME=nhentai-dl
set PACKAGE_VERSION=0.1.0.0
set CONFIGURATION=Release

set MSBUILD=msbuild.exe
set MSBUILDFLAGS=/p:Configuration=%CONFIGURATION%
set RMDIR=del /s /q
set MKDIR=md
set NSIS=makensis.exe

set ERROR=0

if "%1" == "/?" goto help
if "%1" == "/h" goto help
if "%1" == "-h" goto help
if "%1" == "--help" goto help
if "%1" == "help" goto help

if "%1" == "" goto build
if "%1" == "build" goto build
if "%1" == "clean" goto clean
if "%1" == "nsis" goto nsis
if "%1" == "/help" goto help

echo invalid command "%1".
goto end

:build
	for %%X in (%MSBUILD%) do (set MSBUILD_FOUND=%%~$PATH:X)
	if not defined MSBUILD_FOUND goto no_msbuild
	
	%MSBUILD% %MSBUILDFLAGS% NHentai-DL.sln
	set ERROR=%errorlevel%
	if not "%ERROR%" == "0" goto error
	goto end

:clean
	for %%X in (%MSBUILD%) do (set MSBUILD_FOUND=%%~$PATH:X)
	if not defined MSBUILD_FOUND goto no_msbuild
	
	%MSBUILD% %MSBUILDFLAGS% /t:clean NHentai-DL.sln
	set ERROR=%errorlevel%
	if not "%ERROR%" == "0" goto error
	%RMDIR% bin obj dist doc
	set ERROR=%errorlevel%
	if not "%ERROR%" == "0" goto error
	goto end

:nsis
	for %%X in (%NSIS%) do (set NSIS_FOUND=%%~$PATH:X)
	if not defined NSIS_FOUND goto no_nsis
	
	if not exist dist %MKDIR% dist
	set ERROR=%errorlevel%
	if not "%ERROR%" == "0" goto error
	%NSIS% NHentai-DL.nsi
	set ERROR=%errorlevel%
	if not "%ERROR%" == "0" goto error
	goto end

:help
	echo usage: %0 ^<build^|clean^|nsis^>
	goto end

:error
	if "%ERROR%" == "0" goto end
	echo.
	echo Error: %ERROR%

:no_msbuild
	echo "%MSBUILD%" not found.
	goto end

:no_nsis
	echo "%NSIS%" not found.
	goto end

:end
