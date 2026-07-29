#!/usr/bin/env pwsh
# Runs the interactive Cocos2D-Mono test app.
#
# Usage:
#   ./run-testapp.ps1                       # DesktopGL (repo base TFM), Debug
#   ./run-testapp.ps1 -Tfm net10.0-windows7.0   # WindowsDX (see the TFM dials in Directory.Build.props)
#   ./run-testapp.ps1 -Configuration Release
param(
    [string]$Tfm,
    [string]$Configuration = "Debug"
)

$ErrorActionPreference = "Stop"

if (-not $Tfm) {
    # Default to the repo's base TFM dial so this script can't drift when
    # Directory.Build.props bumps .NET versions (it shipped hardcoded to net9.0
    # once and went stale at the .NET 10 retarget).
    $props = Join-Path $PSScriptRoot ".." "Directory.Build.props"
    $match = Select-String -Path $props -Pattern '<Cocos2DBaseTfm[^>]*>([^<]+)</Cocos2DBaseTfm>' | Select-Object -First 1
    if (-not $match) { throw "Could not resolve Cocos2DBaseTfm from $props" }
    $Tfm = $match.Matches[0].Groups[1].Value
}

$project = Join-Path $PSScriptRoot "Cocos2DMono.IntegrationTests/Cocos2DMono.IntegrationTests.csproj"

Write-Host "Running the test app ($Tfm, $Configuration)..." -ForegroundColor Cyan
dotnet run --project $project -f $Tfm -c $Configuration -p:TargetFrameworks=$Tfm
