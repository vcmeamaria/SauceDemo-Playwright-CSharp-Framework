# SauceDemo Playwright Framework — C#

A production-style UI automation framework for the SauceDemo smoke and regression packs.

## Technology

- .NET 8
- Playwright for .NET 1.60.0
- NUnit 4
- Page Object Model
- Serilog
- Allure NUnit
- Parallel fixture execution
- Failure screenshots and Playwright traces
- Environment-variable overrides

## Structure

```text
Config/       Configuration loading
Models/       Test-data models
Pages/        Page Objects and reusable components
Tests/        Smoke and regression tests
Utilities/    Logging and artifact paths
artifacts/    Runtime logs, traces, screenshots and reports
```

## Setup

```bash
dotnet restore
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install
```

On Linux/macOS, use the generated Playwright installation script appropriate to your shell.

## Run

```bash
dotnet test --settings nunit.runsettings
dotnet test --filter "TestCategory=Smoke"
dotnet test --filter "TestCategory=Regression"
dotnet test --filter "TestCategory=Login"
```

Run headed:

```bash
HEADLESS=false dotnet test --filter "TestCategory=Smoke"
```

Use a different browser or URL:

```bash
BROWSER=firefox BASE_URL=https://www.saucedemo.com/ dotnet test
```

## Allure

```bash
allure generate artifacts/allure-results --clean -o artifacts/allure-report
allure open artifacts/allure-report
```

## CI recommendation

Execute smoke tests on every pull request and the complete regression suite nightly or before release. Publish:

- `artifacts/test-results`
- `artifacts/screenshots`
- `artifacts/traces`
- `artifacts/logs`
- `artifacts/allure-results`

## Notes

Each test receives an isolated browser context. Tests use Playwright's auto-waiting locators rather than fixed sleeps. Test IDs from the manual test pack are represented as NUnit categories.
