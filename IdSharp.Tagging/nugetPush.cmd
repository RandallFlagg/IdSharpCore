@echo off
set DEPLOY_DIR=%CD%\..\Nuget Deploy
set TAGGING_VERSION=1.0.0-rc4
set TAGGING_NUPKG_FILE=%CD%\bin\release\IdSharp.Tagging.%TAGGING_VERSION%.nupkg

dotnet pack "%CD%\IdSharp.Tagging-core.csproj" -c release
::Release or release?
mkdir "%DEPLOY_DIR%"
copy "%TAGGING_NUPKG_FILE%" "%DEPLOY_DIR%"
dotnet nuget push "%TAGGING_NUPKG_FILE%" -k %1 -s https://api.nuget.org/v3/index.json

:EXIT