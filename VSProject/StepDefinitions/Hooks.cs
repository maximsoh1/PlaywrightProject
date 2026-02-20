using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Reqnroll;
using Reqnroll.BoDi;
using Allure.Net.Commons;

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
            // Start Allure test case
            var testResult = new TestResult
            {
                uuid = Guid.NewGuid().ToString(),
                name = _scenarioContext.ScenarioInfo.Title,
                fullName = $"{_featureContext.FeatureInfo.Title}.{_scenarioContext.ScenarioInfo.Title}",
                labels = new System.Collections.Generic.List<Label>
                {
                    Label.Feature(_featureContext.FeatureInfo.Title),
                    Label.Suite(_featureContext.FeatureInfo.Title),
                    Label.Story(_scenarioContext.ScenarioInfo.Title)
                }
            };

            // Add tags as labels
            foreach (var tag in _scenarioContext.ScenarioInfo.Tags)
            {
                testResult.labels.Add(Label.Tag(tag));
            }

            AllureLifecycle.Instance.StartTestCase(testResult);

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
        [AfterScenario(Order = 99)]
        public async Task AfterScenario()
        {
            // Update Allure test status
            if (_scenarioContext.TestError != null)
            {
                AllureLifecycle.Instance.UpdateTestCase(tc =>
                {
                    tc.status = Status.failed;
                    tc.statusDetails = new StatusDetails
                    {
                        message = _scenarioContext.TestError.Message,
                        trace = _scenarioContext.TestError.StackTrace
                    };
                });

                // Attach screenshot if available
                if (_page != null)
                {
                    try
                    {
                        var screenshot = await _page.ScreenshotAsync();
                        var screenshotPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "allure-results", $"screenshot-{Guid.NewGuid()}.png");
                        Directory.CreateDirectory(Path.GetDirectoryName(screenshotPath));
                        File.WriteAllBytes(screenshotPath, screenshot);

                        AllureLifecycle.Instance.UpdateTestCase(tc =>
                        {
                            tc.attachments.Add(new Attachment
                            {
                                name = "Screenshot on failure",
                                type = "image/png",
                                source = Path.GetFileName(screenshotPath)
                            });
                        });
                    }
                    catch { /* Ignore screenshot errors */ }
                }
            }
            else
            {
                AllureLifecycle.Instance.UpdateTestCase(tc => tc.status = Status.passed);
            }

            // Stop and write test case
            AllureLifecycle.Instance.StopTestCase();
            AllureLifecycle.Instance.WriteTestCase();

            // Close resources
            await _page?.CloseAsync();
            await _browser?.CloseAsync();
            _playwright?.Dispose();
        }
    }
}
