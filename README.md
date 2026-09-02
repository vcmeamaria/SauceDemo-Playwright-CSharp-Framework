# SauceDemo Playwright C# Framework

A Playwright automation testing framework for the SauceDemo website, built with C#, .NET 8 and NUnit.

The project contains smoke and regression tests covering the main SauceDemo user journeys and uses the Page Object Model to keep test logic organised and reusable.

---

## Technology

- .NET 8
- C#
- Microsoft Playwright for .NET
- NUnit
- Page Object Model
- Serilog
- Allure NUnit
- Chromium
- Firefox
- Parallel test execution
- PowerShell test runner

---

## Framework Structure

```text
SauceDemo.Playwright.CSharp
│
├── Config/
│   └── TestSettings.cs
│
├── Models/
│   └── CheckoutCustomer.cs
│
├── Pages/
│   ├── BasePage.cs
│   ├── LoginPage.cs
│   ├── InventoryPage.cs
│   ├── ProductDetailsPage.cs
│   ├── CartPage.cs
│   ├── CheckoutInformationPage.cs
│   ├── CheckoutOverviewPage.cs
│   ├── CheckoutCompletePage.cs
│   └── MenuComponent.cs
│
├── Tests/
│   ├── BaseTest.cs
│   ├── SmokeTests.cs
│   ├── LoginRegressionTests.cs
│   ├── InventoryRegressionTests.cs
│   └── CartCheckoutRegressionTests.cs
│
├── Utilities/
│   ├── ArtifactPaths.cs
│   ├── LogManager.cs
│   └── VideoManager.cs
│
├── artifacts/
│   ├── logs/
│   ├── screenshots/
│   ├── traces/
│   ├── videos/
│   └── test-results/
│
├── appsettings.json
├── allureConfig.json
├── nunit.runsettings
├── run-tests.ps1
└── SauceDemo.Playwright.CSharp.csproj
```

---

# Setup

## 1. Restore NuGet packages

```bash
dotnet restore
```

## 2. Build the project

```bash
dotnet build
```

## 3. Install Playwright browsers

```bash
pwsh bin/Debug/net8.0/playwright.ps1 install
```

Firefox can also be installed individually with:

```bash
pwsh bin/Debug/net8.0/playwright.ps1 install firefox
```

---

# Running Tests

The recommended way to run the framework is with:

```bash
pwsh ./run-tests.ps1
```

By default, this runs the tests using Chromium.

---

## Run with Chromium

```bash
pwsh ./run-tests.ps1 -Browser chromium
```

---

## Run with Firefox

```bash
pwsh ./run-tests.ps1 -Browser firefox
```

The same test suite is used for both browsers.

The framework has been verified with:

```text
Chromium
40 passed
0 failed

Firefox
40 passed
0 failed
```

---

# Parallel Execution

The framework uses NUnit parallel execution.

The configuration is stored in:

```text
nunit.runsettings
```

The framework currently uses:

```text
4 NUnit test workers
```

Tests use isolated Playwright browser contexts so they can run independently.

Run the parallel suite normally with:

```bash
pwsh ./run-tests.ps1 -Browser chromium
```

or:

```bash
pwsh ./run-tests.ps1 -Browser firefox
```

---

# Run Tests Directly with dotnet

The suite can also be run without the PowerShell runner:

```bash
dotnet test --settings nunit.runsettings
```

However, `run-tests.ps1` is recommended because it automatically creates timestamped TRX result files.

---

# Test Categories

The framework contains 40 automated tests covering areas including:

- Login
- Invalid and locked-user login behaviour
- Inventory
- Product details
- Product sorting
- Cart behaviour
- Adding and removing products
- Checkout validation
- Checkout overview
- Order completion
- Navigation
- Logout
- Application-state reset

The test suite contains both:

```text
Smoke tests
Regression tests
```

---

# Configuration

Framework configuration is stored in:

```text
appsettings.json
```

Example settings include:

```json
{
  "baseUrl": "https://www.saucedemo.com/",
  "browser": "chromium",
  "headless": true,
  "recordVideo": true,
  "traceOnFailure": true,
  "screenshotOnFailure": true
}
```

Environment variables can override selected settings.

Examples:

```text
BASE_URL
BROWSER
HEADLESS
```

---

# Headed Mode

The framework normally runs headlessly.

To display the browser while the tests run from Git Bash:

```bash
HEADLESS=false pwsh ./run-tests.ps1 -Browser chromium
```

This is useful when demonstrating the automation.

---

# Page Object Model

The framework uses the Page Object Model.

Tests describe the scenario:

```csharp
await LoginAsAsync();
await InventoryPage.AddProductAsync("Sauce Labs Backpack");
await InventoryPage.AssertCartCountAsync(1);
```

Page Objects contain the locators and browser interactions.

SauceDemo uses stable `data-test` attributes, which are accessed explicitly by the framework to support reliable parallel execution.

---

# Automatic Waiting

The framework uses Playwright's built-in waiting and assertions.

Fixed delays such as:

```csharp
Thread.Sleep(...)
```

are not used.

Playwright automatically waits for elements to become actionable before interacting with them.

---

# Artifacts

Generated test artifacts are stored under:

```text
artifacts/
```

These files are generated automatically and are excluded from Git.

---

## Logs

```text
artifacts/logs/
```

Log files include the date and time:

```text
playwright-20260902-141941.log
```

Logs include framework actions such as:

```text
Fill: username = standard_user
Fill: password = ***
Click: Login button
Click: Add Sauce Labs Backpack
```

Browser console messages and browser page errors are also logged.

---

## Screenshots

Failure screenshots are stored in:

```text
artifacts/screenshots/
```

Screenshot filenames contain:

```text
TestName-Date-Time.png
```

Example:

```text
LoginPageLoads-20260902-100714-123.png
```

Screenshots are only created when a test fails.

Failure screenshots are also attached to the Allure report.

---

## Playwright Traces

Failure traces are stored in:

```text
artifacts/traces/
```

Example:

```text
LoginPageLoads-20260902-100714-123.zip
```

Traces are only saved when a test fails.

They can be used to investigate:

- Browser actions
- DOM snapshots
- Screenshots
- Network activity
- Test execution steps

---

## Videos

Videos are stored in:

```text
artifacts/videos/
```

Video filenames contain the test name, date and time.

Example:

```text
UserCanLogout-20260902-141314-784.webm
```

The framework keeps a maximum of:

```text
3 videos
```

When more videos are created, the oldest completed videos are automatically removed.

This prevents the artifacts directory from continually increasing in size.

---

# Test Results

Timestamped TRX results are stored in:

```text
artifacts/test-results/
```

Running:

```bash
pwsh ./run-tests.ps1 -Browser chromium
```

creates a result such as:

```text
test-results-chromium-20260902-143602.trx
```

Firefox creates:

```text
test-results-firefox-20260902-143755.trx
```

This makes it easier to identify which browser and test run produced each result.

---

# Allure Report

The framework uses Allure for test reporting.

After running tests, start the report with:

```bash
allure serve bin/Debug/net8.0/allure-results
```

The report displays information including:

- Passed tests
- Failed tests
- Test duration
- Test categories
- Failure information
- Failure screenshot attachments

Example result:

```text
40 test cases
100% passed
```

---

# Git Workflow

Development is completed using feature branches.

The general workflow is:

```text
main
 ↓
pull latest changes
 ↓
create feature branch
 ↓
make changes
 ↓
run tests
 ↓
commit
 ↓
push branch
 ↓
create pull request
 ↓
merge into main
 ↓
pull updated main
```

Example commands:

```bash
git checkout main
git pull
```

Create a branch:

```bash
git checkout -b feature/example
```

Push the new branch:

```bash
git push -u origin feature/example
```

After making and testing changes:

```bash
git add .
git commit -m "Describe the change"
git push
```

After the pull request is merged:

```bash
git checkout main
git pull
```

---

# Reliability Features

The framework includes:

- Page Object Model
- Reusable components
- Stable SauceDemo `data-test` locators
- Playwright automatic waiting
- Isolated browser contexts
- Parallel NUnit execution
- Chromium and Firefox support
- Screenshot capture on failure
- Playwright traces on failure
- Browser console logging
- Timestamped logs
- Timestamped test results
- Timestamped videos
- Three-video retention limit
- Allure reporting
- Password masking in logs

---

# Project Scope

This version of the framework focuses on UI automation testing.

Included:

```text
UI automation
Smoke testing
Regression testing
Cross-browser testing
Parallel execution
Diagnostics
Artifacts
Reporting
```

The following advanced areas are intentionally outside the scope of this project:

```text
API-assisted testing
Network mocking
GitHub Actions
CI/CD pipeline
```

---

# Final Verification

Before merging final changes into `main`, run:

```bash
pwsh ./run-tests.ps1 -Browser chromium
```

and:

```bash
pwsh ./run-tests.ps1 -Browser firefox
```

Expected result:

```text
Total: 40
Passed: 40
Failed: 0
```