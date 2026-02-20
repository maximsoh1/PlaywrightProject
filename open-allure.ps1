# Open Allure Report (without running tests)
# Usage: Run tests in Test Explorer first, then run this script

Write-Host "`n📊 Opening Allure Report..." -ForegroundColor Cyan

$allureResults = "VSProject/bin/Release/net9.0/allure-results"

# Check for Debug results if Release not found
if (-Not (Test-Path $allureResults)) {
    $allureResults = "VSProject/bin/Debug/net9.0/allure-results"
    if (-Not (Test-Path $allureResults)) {
        Write-Host "❌ No test results found!" -ForegroundColor Red
        Write-Host "   Please run tests in Test Explorer first" -ForegroundColor Yellow
        Read-Host "Press Enter to exit"
        exit 1
    }
}

$resultFiles = Get-ChildItem -Path $allureResults -Filter "*-result.json" -ErrorAction SilentlyContinue

if ($resultFiles.Count -eq 0) {
    Write-Host "❌ No test results found!" -ForegroundColor Red
    Write-Host "   Please run tests in Test Explorer first" -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "✅ Found $($resultFiles.Count) test result(s)" -ForegroundColor Green
Write-Host "🌐 Starting Allure server..." -ForegroundColor Cyan
Write-Host "   ℹ️  Press Ctrl+C to stop when done`n" -ForegroundColor DarkGray

allure serve $allureResults
