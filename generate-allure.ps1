# Generate Fresh Allure Report
# Usage: .\generate-allure.ps1

Write-Host "`n🚀 Generating Allure Report..." -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray

Write-Host "🧹 Cleaning..." -ForegroundColor Yellow -NoNewline
Remove-Item -Path "VSProject/bin/Release/net9.0/allure-results/*" -Force -Recurse -ErrorAction SilentlyContinue | Out-Null
Remove-Item -Path "allure-report" -Force -Recurse -ErrorAction SilentlyContinue | Out-Null
Write-Host " ✓" -ForegroundColor Green

Write-Host "🏗️  Building..." -ForegroundColor Yellow -NoNewline
$buildOutput = dotnet build VSProject/VSProject.csproj --configuration Release --verbosity quiet 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host " ✗" -ForegroundColor Red
    Write-Host "`n❌ Build failed!" -ForegroundColor Red
    Write-Host $buildOutput
    exit 1
}
Write-Host " ✓" -ForegroundColor Green

Write-Host "🧪 Running tests..." -ForegroundColor Yellow -NoNewline
$testOutput = dotnet test VSProject/VSProject.csproj --configuration Release --no-build --verbosity quiet 2>&1
$testSummary = $testOutput | Select-String "Total tests:|Passed:|Failed:"
Write-Host " ✓" -ForegroundColor Green
Write-Host "   $($testSummary -join ', ')" -ForegroundColor DarkGray

Write-Host "📊 Generating report..." -ForegroundColor Yellow -NoNewline
$allureResults = "VSProject/bin/Release/net9.0/allure-results"

# Check if results exist
if (-Not (Test-Path $allureResults)) {
    Write-Host " ✗" -ForegroundColor Red
    Write-Host "`n❌ No test results found!" -ForegroundColor Red
    Write-Host "   Make sure tests ran successfully" -ForegroundColor DarkGray
    exit 1
}

$resultFiles = Get-ChildItem -Path $allureResults -Filter "*-result.json"
if ($resultFiles.Count -eq 0) {
    Write-Host " ✗" -ForegroundColor Red
    Write-Host "`n❌ No test results in allure-results folder!" -ForegroundColor Red
    exit 1
}

Write-Host " ✓" -ForegroundColor Green
Write-Host "   Found $($resultFiles.Count) test result(s)" -ForegroundColor DarkGray

Write-Host "🌐 Starting web server..." -ForegroundColor Yellow -NoNewline
try {
    Write-Host " ✓" -ForegroundColor Green
    Write-Host "`n📂 Opening Allure Report in browser..." -ForegroundColor Cyan
    Write-Host "   ℹ️  Press Ctrl+C to stop the server when done" -ForegroundColor DarkGray
    Write-Host ""

    # Use allure serve which starts a web server and opens browser
    allure serve $allureResults

} catch {
    Write-Host " ✗" -ForegroundColor Red
    Write-Host "`n❌ Failed to start Allure server!" -ForegroundColor Red
    Write-Host "   Error: $_" -ForegroundColor DarkGray
}

Write-Host "`n✨ Done!`n" -ForegroundColor Green

