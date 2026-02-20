# Open Allure Report (without running tests)
# Usage: Run tests in Test Explorer first, then run this script

Write-Host "`n📊 Opening Allure Report..." -ForegroundColor Cyan

# Try Release first, then Debug
$allureResults = $null
$possiblePaths = @(
    "VSProject/bin/Release/net9.0/allure-results",
    "VSProject/bin/Debug/net9.0/allure-results",
    "VSProject/bin/Release/net10.0/allure-results",
    "VSProject/bin/Debug/net10.0/allure-results"
)

foreach ($path in $possiblePaths) {
    if (Test-Path $path) {
        $resultFiles = Get-ChildItem -Path $path -Filter "*-result.json" -ErrorAction SilentlyContinue
        if ($resultFiles.Count -gt 0) {
            $allureResults = $path
            Write-Host "✅ Found $($resultFiles.Count) test result(s) in:" -ForegroundColor Green
            Write-Host "   $path" -ForegroundColor DarkGray
            break
        }
    }
}

if ($null -eq $allureResults) {
    Write-Host "❌ No test results found!" -ForegroundColor Red
    Write-Host "`n💡 Please run tests first:" -ForegroundColor Yellow
    Write-Host "   1. Open Test Explorer (Ctrl+E, T)" -ForegroundColor DarkGray
    Write-Host "   2. Click 'Run All' or run specific tests" -ForegroundColor DarkGray
    Write-Host "   3. Wait for tests to complete" -ForegroundColor DarkGray
    Write-Host "   4. Run this script again" -ForegroundColor DarkGray
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "🌐 Starting Allure server..." -ForegroundColor Cyan
Write-Host "   ℹ️  Press Ctrl+C to stop when done`n" -ForegroundColor DarkGray

allure serve $allureResults
