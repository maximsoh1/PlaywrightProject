using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Reqnroll;
using Reqnroll.BoDi;

namespace VSProject.StepDefinitions
{
    /// <summary>
    /// Hooks for initializing and cleaning up scenarios
    /// </summary>
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _objectContainer;
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _page;
        
        private static bool _allureResultsCleared = false;

        // Dependency Injection: Reqnroll provides IObjectContainer
        public Hooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
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
        [BeforeScenario]
        public async Task BeforeScenario()
        {
            // Create Playwright instance
            _playwright = await Playwright.CreateAsync();

            // Check if running in CI environment
            bool isCI = Environment.GetEnvironmentVariable("CI") == "true" ||
                       Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true";

            _browser = await _playwright.Chromium.LaunchAsync(new()
            {
                Headless = isCI // true in CI, false locally
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
        [AfterScenario]
        public async Task AfterScenario()
        {
            // Close resources
            await _page?.CloseAsync();
            await _browser?.CloseAsync();
            _playwright?.Dispose();
        }
    }
}
