using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace VSProject.Tests.PlaywrightDev
{
    /// <summary>
    /// Base class for all Playwright tests
    /// Provides video recording and artifact capture on test failure
    /// </summary>
    public abstract class PlaywrightTestBase : PageTest
    {
        /// <summary>
        /// Configure browser context with video recording
        /// </summary>
        public override BrowserNewContextOptions ContextOptions()
        {
            return new BrowserNewContextOptions
            {
                RecordVideoDir = "videos/",
                RecordVideoSize = new() { Width = 1280, Height = 720 }
            };
        }

        /// <summary>
        /// Capture screenshots and videos on test failure
        /// </summary>
        [TearDown]
        public async Task BaseTearDown()
        {
            var testResult = TestContext.CurrentContext.Result.Outcome.Status;
            var testName = TestContext.CurrentContext.Test.Name;
            var dateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var folderPath = Path.Combine("artifacts", $"{testName}_{dateTime}");

            TestContext.WriteLine($"Test status: {testResult}");
            TestContext.WriteLine($"Working directory: {Path.GetFullPath(".")}");
            TestContext.WriteLine($"Videos directory: {Path.GetFullPath("videos")}");

            if (testResult == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                Directory.CreateDirectory(folderPath);
                TestContext.WriteLine($"Artifacts folder: {Path.GetFullPath(folderPath)}");

                // Save screenshot
                await SaveScreenshotAsync(folderPath);

                // Close page to finalize video
                await ClosePageAsync();
                await Task.Delay(1000);

                // Save video
                await SaveVideoAsync(folderPath);
            }
        }

        private async Task SaveScreenshotAsync(string folderPath)
        {
            try
            {
                await Page.ScreenshotAsync(new()
                {
                    Path = Path.Combine(folderPath, "screenshot.png"),
                    FullPage = true
                });
                TestContext.WriteLine("Screenshot saved");
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Failed to save screenshot: {ex.Message}");
            }
        }

        private async Task ClosePageAsync()
        {
            try
            {
                await Page.CloseAsync();
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Failed to close page: {ex.Message}");
            }
        }

        private async Task SaveVideoAsync(string folderPath)
        {
            try
            {
                if (Page.Video != null)
                {
                    var videoPath = await Page.Video.PathAsync();
                    
                    if (!string.IsNullOrEmpty(videoPath) && File.Exists(videoPath))
                    {
                        var targetPath = Path.Combine(folderPath, "video.webm");
                        File.Copy(videoPath, targetPath, overwrite: true);
                        TestContext.WriteLine($"Video saved to: {Path.GetFullPath(targetPath)}");
                    }
                    else
                    {
                        TestContext.WriteLine($"Video path is empty or file does not exist: {videoPath}");
                    }
                }
                else
                {
                    TestContext.WriteLine("Page.Video is null - video recording was not enabled");
                }
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Failed to save video: {ex.Message}");
                TestContext.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}

