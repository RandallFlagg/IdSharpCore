function Confirm-And-RemoveFolder {
    param (
        [Parameter(Mandatory)][string] $FolderPath,
        [string] $Prompt = "Do you want to delete the folder '$FolderPath'?"
    )

    if (-not (Test-Path $FolderPath)) {
        return
    }

    $response = Read-Host "🧹 $Prompt (y/n)"
    if ($response.Trim() -match '^[Yy]$') {
        Remove-Item -Recurse -Force -Path $FolderPath
        Write-Host "🗑️  Deleted folder: $FolderPath"
    } else {
        Write-Host "📦 Folder preserved: $FolderPath"
    }
}

# === Configuration ===
$Projects = @("IdSharp.Common", "IdSharp.Tagging", "IdSharp.AudioInfo") # Add/remove project names as needed
$OutputDir = "NuGetPackages"
$Configuration = "Release"
$NuGetSource = "https://api.nuget.org/v3/index.json"

# === Ask user if they want to start with a clean folder ===
Confirm-And-RemoveFolder -FolderPath $OutputDir -Prompt "Delete existing '$OutputDir' folder before building?"

# === 🔐 Check for API key file ===
if ($args.Count -lt 1) {
    Write-Host "❌ No API key file provided. Usage: .\pack-and-push-nuget.ps1 <API_KEY_FILE>" -ForegroundColor Red
    exit 1
}

$ApiKeyFile = $args[0]
if (-not (Test-Path $ApiKeyFile)) {
    Write-Host "❌ API key file not found: $ApiKeyFile" -ForegroundColor Red
    exit 1
}

$ApiKey = Get-Content -Path $ApiKeyFile -Raw
Write-Host "🔍 Using API key from file: $ApiKeyFile"

# === Create output directory ===
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
}

# === Loop through each project ===
foreach ($proj in $Projects) {
    $projFile = "../$proj/$proj.csproj"
    
    if (-not (Test-Path $projFile)) {
        Write-Host "⚠️ Skipping: $projFile not found." -ForegroundColor Yellow
        continue
    }

    Write-Host "🔧 Building $proj..."
    dotnet build $projFile -c $Configuration
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Build failed for $proj" -ForegroundColor Red
        exit 1
    }

    Write-Host "📦 Packing $proj..."
    dotnet pack $projFile -c $Configuration -o $OutputDir --no-build
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Pack failed for $proj" -ForegroundColor Red
        exit 1
    }
}

# === Push all .nupkg files ===
Get-ChildItem -Path $OutputDir -Filter *.nupkg | ForEach-Object {
    Write-Host "🚀 Pushing $($_.Name)..."
    dotnet nuget push $_.FullName --api-key $ApiKey --source $NuGetSource --skip-duplicate --no-service-endpoint
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Push failed for $($_.Name)" -ForegroundColor Red
        exit 1
    }
}

Write-Host "✅ All NuGet packages pushed successfully!" -ForegroundColor Green

# === clean up output directory ===
# === Ask if user wants to delete the output folder ===
Confirm-And-RemoveFolder -FolderPath $OutputDir -Prompt "Delete '$OutputDir' folder now that packing is complete?"