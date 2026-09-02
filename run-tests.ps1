# ==========================================
# SauceDemo Playwright C# Test Runner
# ==========================================

$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"

$resultsDirectory = Join-Path `
    $PSScriptRoot `
    "artifacts/test-results"

$runSettings = Join-Path `
    $PSScriptRoot `
    "nunit.runsettings"

# Make sure the test-results folder exists.
New-Item `
    -ItemType Directory `
    -Force `
    -Path $resultsDirectory `
    | Out-Null

$resultFile = "test-results-$timestamp.trx"

Write-Host ""
Write-Host "Running SauceDemo Playwright tests..."
Write-Host "Results file: $resultFile"
Write-Host ""

dotnet test `
    --settings $runSettings `
    --results-directory $resultsDirectory `
    --logger "trx;LogFileName=$resultFile"

exit $LASTEXITCODE