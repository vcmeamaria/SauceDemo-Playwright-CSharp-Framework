# ==========================================
# SauceDemo Playwright C# Test Runner
# ==========================================

param(
    [ValidateSet("chromium", "firefox")]
    [string]$Browser = "chromium"
)

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

# Tell Playwright which browser to use.
$env:BROWSER = $Browser

$resultFile = "test-results-$Browser-$timestamp.trx"

Write-Host ""
Write-Host "Running SauceDemo Playwright tests..."
Write-Host "Browser: $Browser"
Write-Host "Results file: $resultFile"
Write-Host ""

dotnet test `
    --settings $runSettings `
    --results-directory $resultsDirectory `
    --logger "trx;LogFileName=$resultFile"

$exitCode = $LASTEXITCODE

# Remove the temporary environment variable after the run.
Remove-Item Env:BROWSER -ErrorAction SilentlyContinue

exit $exitCode