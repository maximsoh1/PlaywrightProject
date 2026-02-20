using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Reqnroll;
using Reqnroll.BoDi;
using System.Text.Json;
using System.Collections.Generic;

namespace VSProject.StepDefinitions
{
    /// <summary>
    /// Hooks for initializing and cleaning up scenarios
    /// </summary>
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _objectContainer;
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _page;
        private DateTime _scenarioStartTime;
        private string _testUuid;

        private static bool _allureResultsCleared = false;

        // Dependency Injection: Reqnroll provides IObjectContainer, ScenarioContext and FeatureContext
        public Hooks(IObjectContainer objectContainer, ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            _objectContainer = objectContainer;
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
        }

        /// <summary>
        /// Executes once before all test runs - clears old Allure results
        /// </summary>
        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            if (!_allureResultsCleared)
            {
                ClearAllureResults();
                _allureResultsCleared = true;
            }
        }

        /// <summary>
        /// Clear old Allure results to avoid duplicates in reports
        /// </summary>
        private static void ClearAllureResults()
        {
            var resultsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "allure-results");
            
            if (Directory.Exists(resultsPath))
            {
                try
                {
                    var files = Directory.GetFiles(resultsPath);
                    foreach (var file in files)
                    {
                        File.Delete(file);
                    }
                    Console.WriteLine($"? Cleared {files.Length} old Allure result files");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"! Warning: Could not clear Allure results: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Executes before each scenario
        /// </summary>
        [BeforeScenario(Order = 1)]
        public async Task BeforeScenario()
        {
            _testUuid = Guid.NewGuid().ToString();
            _scenarioStartTime = DateTime.UtcNow;

            // Create Playwright instance
            _playwright = await Playwright.CreateAsync();

            // Always use headless mode to avoid browser windows staying open
            _browser = await _playwright.Chromium.LaunchAsync(new()
            {
                Headless = true // Always headless - no browser windows!
            });

            // Create context with video recording
            var context = await _browser.NewContextAsync(new()
            {
                RecordVideoDir = "videos/",
                RecordVideoSize = new() { Width = 1280, Height = 720 }
            });

            _page = await context.NewPageAsync();

            // Register IPage in DI container
            // Now all Step Definitions can receive this Page via constructor
            _objectContainer.RegisterInstanceAs(_page);
        }

        /// <summary>
        /// Executes after each scenario
        /// </summary>
        [AfterScenario(Order = 99)]
        public async Task AfterScenario()
        {
            try
            {
                var stop = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var start = new DateTimeOffset(_scenarioStartTime).ToUnixTimeMilliseconds();

                // Create Allure result JSON manually
                var allureResult = new
                {
                    uuid = _testUuid,
                    historyId = $"{_featureContext.FeatureInfo.Title}.{_scenarioContext.ScenarioInfo.Title}",
                    fullName = $"{_featureContext.FeatureInfo.Title}.{_scenarioContext.ScenarioInfo.Title}",
                    name = _scenarioContext.ScenarioInfo.Title,
                    status = _scenarioContext.TestError == null ? "passed" : "failed",
                    statusDetails = _scenarioContext.TestError != null ? new
                    {
                        message = _scenarioContext.TestError.Message,
                        trace = _scenarioContext.TestError.StackTrace
                    } : null,
                    start,
                    stop,
                    labels = new[]
                    {
                        new { name = "feature", value = _featureContext.FeatureInfo.Title },
                        new { name = "suite", value = _featureContext.FeatureInfo.Title },
                        new { name = "story", value = _scenarioContext.ScenarioInfo.Title },
                        new { name = "framework", value = "reqnroll" },
                        new { name = "language", value = "C#" }
                    }
                };

                // Write to allure-results directory
                var resultsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "allure-results");
                Directory.CreateDirectory(resultsPath);

                var resultFileName = Path.Combine(resultsPath, $"{_testUuid}-result.json");
                var json = JsonSerializer.Serialize(allureResult, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(resultFileName, json);

                Console.WriteLine($"✓ Allure result saved: {Path.GetFileName(resultFileName)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not save Allure result: {ex.Message}");
            }
            finally
            {
                // Always close resources
                try
                {
                    if (_page != null)
                        await _page.CloseAsync();
                    if (_browser != null)
                        await _browser.CloseAsync();
                    _playwright?.Dispose();
                }
                catch
                {
                    /* Ignore cleanup errors */
                }
            }
        }
    }
}
