@echo off
setlocal EnableDelayedExpansion

:: === Configuration ===
set PROJECT_FILE=IdSharp.Tagging-core.csproj
set OUTPUT_DIR=NuGetPackages
set CONFIGURATION=Release
set NUGET_SOURCE=https://api.nuget.org/v3/index.json

::set "API_KEY="

:: 🔐 Check if an API key file was provided
if "%~1"=="" (
    echo ❌ No API key provided. Usage: pack-and-push-nuget.bat ^<API_KEY_FILE^>
    exit /b 1
)

:: 🗂️ Now check that the file actually exists
if not exist "%~1" (
    echo ❌ File not found: "%~1"
    exit /b 1
)

set "API_KEY_FILE=%~1"
echo 🔍 Using API key file: "%API_KEY_FILE%"

set /p API_KEY=<"%API_KEY_FILE%"

:: === Build the project ===
echo Building project...
dotnet build %PROJECT_FILE% -c %CONFIGURATION%
if errorlevel 1 (
    echo ❌ Build failed!
    exit /b 1
)

:: === Pack the project ===
echo Packing NuGet package...
dotnet pack %PROJECT_FILE% -c %CONFIGURATION% -o %OUTPUT_DIR% --no-build
if errorlevel 1 (
    echo ❌ Pack failed!
    exit /b 1
)

echo ✅ NuGet package created in "%OUTPUT_DIR%"

:: === Push the package ===
echo Pushing NuGet package...
    :: --skip-duplicate
    :: -NonInteractive
    :: -SkipDuplicate
    @REM nuget push "%%f" -ApiKey %API_KEY% -Source %NUGET_SOURCE%
for %%f in (%OUTPUT_DIR%\*.nupkg) do (
    echo → Pushing %%~nxf
    :: dotnet nuget push "%%f" --api-key %API_KEY% --source %NUGET_SOURCE%
    if errorlevel 1 (
        echo ❌ Push failed for %%~nxf
        exit /b 1
    )
)

echo ✅ All packages pushed successfully!

echo Cleaning up...
REM Delete the folder before exiting
:: if exist "%OUTPUT_DIR%" rmdir /s "%OUTPUT_DIR%"

:EXIT
endlocal
