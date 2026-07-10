#!/usr/bin/env pwsh
# Runs the interactive Cocos2D-Mono test app.
#
# Usage:
#   ./run-testapp.ps1                       # DesktopGL (net9.0), Debug
#   ./run-testapp.ps1 -Tfm net9.0-windows7.0
#   ./run-testapp.ps1 -Configuration Release
param(
    [string]$Tfm = "net9.0",
    [string]$Configuration = "Debug"
)

$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "Cocos2DMono.IntegrationTests/Cocos2DMono.IntegrationTests.csproj"

Write-Host "Running the test app ($Tfm, $Configuration)..." -ForegroundColor Cyan
dotnet run --project $project -f $Tfm -c $Configuration -p:TargetFrameworks=$Tfm
