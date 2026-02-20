# Open Allure Report (without running tests)
# Usage: Run tests in Test Explorer first, then run this script

Write-Host "`n📊 Opening Allure Report..." -ForegroundColor Cyan

# Try Debug first (Test Explorer default), then Release
$allureResults = ""
$debugPath = "VSProject/bin/Debug/net9.0/allure-results"
$releasePath = "VSProject/bin/Release/net9.0/allure-results"

if (Test-Path $debugPath) {
    $debugFiles = Get-ChildItem -Path $debugPath -Filter "*-result.json" -ErrorAction SilentlyContinue
    if ($debugFiles.Count -gt 0) {
        $allureResults = $debugPath
        Write-Host "✅ Found $($debugFiles.Count) test result(s) in Debug" -ForegroundColor Green
    }
}

if ($allureResults -eq "" -and (Test-Path $releasePath)) {
    $releaseFiles = Get-ChildItem -Path $releasePath -Filter "*-result.json" -ErrorAction SilentlyContinue
    if ($releaseFiles.Count -gt 0) {
        $allureResults = $releasePath
        Write-Host "✅ Found $($releaseFiles.Count) test result(s) in Release" -ForegroundColor Green
    }
}

if ($allureResults -eq "") {
    Write-Host "❌ No test results found!" -ForegroundColor Red
    Write-Host "   Please run tests in Test Explorer first" -ForegroundColor Yellow
    Write-Host "`n   📝 How to run tests:" -ForegroundColor Cyan
    Write-Host "   1. Open Test Explorer: View → Test Explorer (Ctrl+E, T)" -ForegroundColor DarkGray
    Write-Host "   2. Click 'Run All Tests' button (▶️)" -ForegroundColor DarkGray
    Write-Host "   3. Wait for tests to complete" -ForegroundColor DarkGray
    Write-Host "   4. Run this script again`n" -ForegroundColor DarkGray
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "🌐 Starting Allure server..." -ForegroundColor Cyan
Write-Host "   ℹ️  Press Ctrl+C to stop when done`n" -ForegroundColor DarkGray

allure serve $allureResults
