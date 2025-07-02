@echo off
setlocal

:: === Configuration ===
set PROJECT_FILE=IdSharp.Tagging-core.csproj
set OUTPUT_DIR=NuGetPackages2
set CONFIGURATION=Release
set NUGET_SOURCE=https://api.nuget.org/v3/index.json
@REM set API_KEY=%1

set "API_KEY="
set "API_KEY_FILE=%1"

if not exist "%API_KEY_FILE%" (
    echo ❌ File not found: %API_KEY_FILE%
    exit /b 1
)

set /p API_KEY=<"%API_KEY_FILE%"

@REM if "%API_KEY%"=="" (
@REM     echo ❌ No API key provided. Usage: pack-and-push-nuget.bat <API_KEY>
@REM     exit /b 1
@REM )

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
    dotnet nuget push "%%f" --api-key %API_KEY% --source %NUGET_SOURCE%
    if errorlevel 1 (
        echo ❌ Push failed for %%~nxf
        exit /b 1
    )
)

echo ✅ All packages pushed successfully!

endlocal

:EXIT

@REM set DEPLOY_DIR=%CD%\..\Nuget Deploy
@REM set TAGGING_VERSION=1.0.0-rc4
@REM set TAGGING_NUPKG_FILE=%CD%\bin\release\IdSharp.Tagging.%TAGGING_VERSION%.nupkg

@REM dotnet pack "%CD%\IdSharp.Tagging-core.csproj" -c release
@REM ::Release or release?
@REM mkdir "%DEPLOY_DIR%"
@REM copy "%TAGGING_NUPKG_FILE%" "%DEPLOY_DIR%"
@REM dotnet nuget push "%TAGGING_NUPKG_FILE%" -k %1 -s https://api.nuget.org/v3/index.json