#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Builds the A10w.Aspire.Hosting.ExternalAks NuGet package and refreshes AppHost restore from the local package source.

.DESCRIPTION
    This script automates the workflow for testing the library as a NuGet package:
    1. Builds and packs A10w.Aspire.Hosting.ExternalAks into artifacts/packages
    2. Ensures sample/nuget.config points to the local package source
    3. Clears NuGet caches and deletes the package from global-packages
    4. Restores AppHost with --force --no-cache to pick up the latest package build

.EXAMPLE
    ./test-local-package.ps1
    Builds the package and forces AppHost to restore the latest local package.
#>

$ErrorActionPreference = 'Stop'

# Paths
$scriptRoot = $PSScriptRoot
$solutionRoot = Split-Path -Parent $scriptRoot
$libraryProject = Join-Path $solutionRoot 'src' 'A10w.Aspire.Hosting.ExternalAks' 'A10w.Aspire.Hosting.ExternalAks.csproj'
$appHostProject = Join-Path $scriptRoot 'A10w.Aspire.Hosting.ExternalAks.AppHost' 'A10w.Aspire.Hosting.ExternalAks.AppHost.csproj'
$outputDir = Join-Path $solutionRoot 'artifacts' 'packages'

# Step 1: Build and pack the library
Write-Host '==> Building and packing A10w.Aspire.Hosting.ExternalAks...' -ForegroundColor Cyan

if (-not (Test-Path $libraryProject)) {
    Write-Error "Library project not found at: $libraryProject"
    exit 1
}

# Ensure output directory exists
if (-not (Test-Path $outputDir)) {
    New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
}

# Pack the library
dotnet pack $libraryProject --configuration Release --output $outputDir
if ($LASTEXITCODE -ne 0) {
    Write-Error 'Failed to pack the library project.'
    exit $LASTEXITCODE
}

# Find the generated package
$packageFiles = Get-ChildItem -Path $outputDir -Filter 'A10w.Aspire.Hosting.ExternalAks.*.nupkg' | Sort-Object LastWriteTime -Descending
if ($packageFiles.Count -eq 0) {
    Write-Error "No package found in output directory: $outputDir"
    exit 1
}

$packagePath = $packageFiles[0].FullName
$packageVersion = $packageFiles[0].Name -replace 'A10w\.Aspire\.Hosting\.ExternalAks\.(.+)\.nupkg', '$1'

Write-Host "✓ Package created: $packagePath" -ForegroundColor Green
Write-Host "  Version: $packageVersion" -ForegroundColor Gray

# Step 2: Add local package source and restore
Write-Host '==> Restoring NuGet packages...' -ForegroundColor Cyan

# Create or update nuget.config in the sample directory to include local source
$nugetConfigPath = Join-Path $scriptRoot 'nuget.config'
$nugetConfigContent = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="local-packages" value="$outputDir" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
"@

Set-Content -Path $nugetConfigPath -Value $nugetConfigContent -Force
Write-Host "✓ Created nuget.config: $nugetConfigPath" -ForegroundColor Green

# Ensure the specific package cannot be reused from global cache.
$globalPackagesPath = Join-Path $HOME '.nuget' 'packages' 'a10w.aspire.hosting.externalaks'

Write-Host '==> Clearing NuGet caches...' -ForegroundColor Cyan
dotnet nuget locals all --clear | Out-Null

if (Test-Path $globalPackagesPath) {
    Remove-Item -Path $globalPackagesPath -Recurse -Force
    Write-Host "✓ Removed global package cache: $globalPackagesPath" -ForegroundColor Green
}

# Restore packages with no cache so latest package is always used.
dotnet restore $appHostProject --force --no-cache
if ($LASTEXITCODE -ne 0) {
    Write-Error 'Failed to restore NuGet packages from local source.'
    exit $LASTEXITCODE
}

Write-Host ''
Write-Host '✓ Successfully refreshed AppHost to use latest local package build!' -ForegroundColor Green
Write-Host ''
Write-Host 'Next steps:' -ForegroundColor Yellow
Write-Host '  1. Run the AppHost to test the package: dotnet run --project A10w.Aspire.Hosting.ExternalAks.AppHost'
Write-Host '  2. Re-run this script after code changes to repack and force-refresh the package'
Write-Host ''
