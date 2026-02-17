using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace VSProject.Tests.Pages.Shopping
{
    /// <summary>
    /// Helper class for visual regression testing using Playwright screenshots
    /// </summary>
    public class VisualTestingHelper
    {
        private readonly IPage _page;
        private readonly string _baselinePath;

        public VisualTestingHelper(IPage page)
        {
            _page = page;
            
            // Baseline screenshots stored in Tests/VisualBaselines
            _baselinePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, 
                "..", "..", "..", 
                "Tests", "VisualBaselines"
            );
            
            // Create directory if not exists
            Directory.CreateDirectory(_baselinePath);
        }

        /// <summary>
        /// Compare full page screenshot with baseline
        /// </summary>
        public async Task<bool> ComparePageScreenshotAsync(string testName, PageScreenshotOptions? options = null)
        {
            var screenshotOptions = options ?? new PageScreenshotOptions
            {
                FullPage = true,
                Animations = ScreenshotAnimations.Disabled,
            };

            var baselineName = $"{testName}.png";
            var baselineFile = Path.Combine(_baselinePath, baselineName);

            try
            {
                // If baseline doesn't exist, create it on first run
                if (!File.Exists(baselineFile))
                {
                    await _page.ScreenshotAsync(new PageScreenshotOptions
                    {
                        Path = baselineFile,
                        FullPage = screenshotOptions.FullPage,
                        Animations = screenshotOptions.Animations
                    });
                    
                    Console.WriteLine($"? Baseline created: {baselineName}");
                    return true; // First run - baseline created
                }

                // Baseline exists - compare with current screenshot
                var actualFile = Path.Combine(_baselinePath, $"{testName}-actual.png");
                
                await _page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = actualFile,
                    FullPage = screenshotOptions.FullPage,
                    Animations = screenshotOptions.Animations
                });

                // Compare files (pixel-by-pixel)
                var filesMatch = CompareImages(baselineFile, actualFile);

                if (filesMatch)
                {
                    // Images match - delete actual file
                    File.Delete(actualFile);
                    Console.WriteLine($"? Screenshot matches baseline: {baselineName}");
                    return true;
                }
                else
                {
                    // Images differ - keep actual file for investigation
                    Console.WriteLine($"? Screenshot differs from baseline: {baselineName}");
                    Console.WriteLine($"  Baseline: {baselineFile}");
                    Console.WriteLine($"  Actual: {actualFile}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Error comparing screenshot: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Compare specific element screenshot with baseline
        /// </summary>
        public async Task<bool> CompareElementScreenshotAsync(ILocator locator, string testName)
        {
            var baselineName = $"{testName}-element.png";
            var baselineFile = Path.Combine(_baselinePath, baselineName);

            try
            {
                // If baseline doesn't exist, create it
                if (!File.Exists(baselineFile))
                {
                    await locator.ScreenshotAsync(new LocatorScreenshotOptions
                    {
                        Path = baselineFile,
                        Animations = ScreenshotAnimations.Disabled
                    });
                    
                    Console.WriteLine($"? Baseline created: {baselineName}");
                    return true;
                }

                // Baseline exists - compare
                var actualFile = Path.Combine(_baselinePath, $"{testName}-element-actual.png");
                
                await locator.ScreenshotAsync(new LocatorScreenshotOptions
                {
                    Path = actualFile,
                    Animations = ScreenshotAnimations.Disabled
                });

                var filesMatch = CompareImages(baselineFile, actualFile);

                if (filesMatch)
                {
                    File.Delete(actualFile);
                    Console.WriteLine($"? Element screenshot matches baseline: {baselineName}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"? Element screenshot differs: {baselineName}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Error comparing element: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Take screenshot with masked elements (to ignore dynamic content)
        /// </summary>
        public async Task<bool> ComparePageWithMaskAsync(string testName, string[] maskedSelectors)
        {
            var baselineName = $"{testName}-masked.png";
            var baselineFile = Path.Combine(_baselinePath, baselineName);

            // Convert selectors to locators
            var maskedLocators = new ILocator[maskedSelectors.Length];
            for (int i = 0; i < maskedSelectors.Length; i++)
            {
                maskedLocators[i] = _page.Locator(maskedSelectors[i]);
            }

            try
            {
                // If baseline doesn't exist, create it
                if (!File.Exists(baselineFile))
                {
                    await _page.ScreenshotAsync(new PageScreenshotOptions
                    {
                        Path = baselineFile,
                        FullPage = true,
                        Animations = ScreenshotAnimations.Disabled,
                        Mask = maskedLocators
                    });
                    
                    Console.WriteLine($"? Baseline created: {baselineName}");
                    return true;
                }

                // Baseline exists - compare
                var actualFile = Path.Combine(_baselinePath, $"{testName}-masked-actual.png");
                
                await _page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = actualFile,
                    FullPage = true,
                    Animations = ScreenshotAnimations.Disabled,
                    Mask = maskedLocators
                });

                var filesMatch = CompareImages(baselineFile, actualFile);

                if (filesMatch)
                {
                    File.Delete(actualFile);
                    Console.WriteLine($"? Masked screenshot matches baseline: {baselineName}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"? Masked screenshot differs: {baselineName}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Error comparing masked screenshot: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Set viewport size for responsive testing
        /// </summary>
        public async Task SetViewportAsync(int width, int height)
        {
            await _page.SetViewportSizeAsync(width, height);
            
            // Wait for layout to stabilize
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Set mobile viewport (iPhone 12)
        /// </summary>
        public async Task SetMobileViewportAsync()
        {
            await SetViewportAsync(390, 844);
        }

        /// <summary>
        /// Set tablet viewport (iPad)
        /// </summary>
        public async Task SetTabletViewportAsync()
        {
            await SetViewportAsync(768, 1024);
        }

        /// <summary>
        /// Set desktop viewport (1920x1080)
        /// </summary>
        public async Task SetDesktopViewportAsync()
        {
            await SetViewportAsync(1920, 1080);
        }

        /// <summary>
        /// Take screenshot of element on hover
        /// </summary>
        public async Task<bool> CompareElementHoverStateAsync(ILocator locator, string testName)
        {
            var baselineName = $"{testName}-hover.png";
            var baselineFile = Path.Combine(_baselinePath, baselineName);

            try
            {
                // Hover over element
                await locator.HoverAsync();
                
                // Wait a bit for hover effects
                await _page.WaitForTimeoutAsync(500);

                // If baseline doesn't exist, create it
                if (!File.Exists(baselineFile))
                {
                    await locator.ScreenshotAsync(new LocatorScreenshotOptions
                    {
                        Path = baselineFile,
                        Animations = ScreenshotAnimations.Disabled
                    });
                    
                    Console.WriteLine($"? Baseline created: {baselineName}");
                    return true;
                }

                // Baseline exists - compare
                var actualFile = Path.Combine(_baselinePath, $"{testName}-hover-actual.png");
                
                await locator.ScreenshotAsync(new LocatorScreenshotOptions
                {
                    Path = actualFile,
                    Animations = ScreenshotAnimations.Disabled
                });

                var filesMatch = CompareImages(baselineFile, actualFile);

                if (filesMatch)
                {
                    File.Delete(actualFile);
                    Console.WriteLine($"? Hover screenshot matches baseline: {baselineName}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"? Hover screenshot differs: {baselineName}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Error comparing hover state: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Compare screenshots with tolerance
        /// Playwright uses pixelmatch algorithm with default threshold
        /// </summary>
        public async Task<bool> ComparePageWithToleranceAsync(string testName, double maxDiffPixelRatio = 0.01)
        {
            var baselineName = $"{testName}-tolerance.png";
            var baselineFile = Path.Combine(_baselinePath, baselineName);

            try
            {
                // Note: This is a placeholder - for production use ImageSharp library
                // with pixel-level comparison and tolerance
                
                if (!File.Exists(baselineFile))
                {
                    await _page.ScreenshotAsync(new PageScreenshotOptions
                    {
                        Path = baselineFile,
                        FullPage = true,
                        Animations = ScreenshotAnimations.Disabled
                    });
                    return true;
                }

                var actualFile = Path.Combine(_baselinePath, $"{testName}-tolerance-actual.png");
                await _page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = actualFile,
                    FullPage = true,
                    Animations = ScreenshotAnimations.Disabled
                });

                var filesMatch = CompareImages(baselineFile, actualFile);
                
                if (filesMatch)
                {
                    File.Delete(actualFile);
                }

                return filesMatch;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Get baseline path for external tools (like Allure)
        /// </summary>
        public string GetBaselinePath()
        {
            return _baselinePath;
        }

        /// <summary>
        /// Check if baseline exists
        /// </summary>
        public bool BaselineExists(string testName)
        {
            var baselineFile = Path.Combine(_baselinePath, $"{testName}.png");
            return File.Exists(baselineFile);
        }

        /// <summary>
        /// Delete baseline (useful for resetting test)
        /// </summary>
        public void DeleteBaseline(string testName)
        {
            var baselineFile = Path.Combine(_baselinePath, $"{testName}.png");
            if (File.Exists(baselineFile))
            {
                File.Delete(baselineFile);
            }
        }

        /// <summary>
        /// Pixel-by-pixel comparison of two images using ImageSharp
        /// Generates a diff image showing differences in red
        /// </summary>
        /// <param name="baselineFile">Path to baseline image</param>
        /// <param name="actualFile">Path to actual (current) image</param>
        /// <param name="tolerance">Percentage of pixels that can differ (default 0.1%)</param>
        /// <returns>True if images are similar within tolerance</returns>
        private bool CompareImages(string baselineFile, string actualFile, double tolerance = 0.1)
        {
            if (!File.Exists(baselineFile) || !File.Exists(actualFile))
            {
                Console.WriteLine($"  One or both files do not exist");
                return false;
            }

            try
            {
                using var baseline = Image.Load<Rgba32>(baselineFile);
                using var actual = Image.Load<Rgba32>(actualFile);

                // Check if dimensions match
                if (baseline.Width != actual.Width || baseline.Height != actual.Height)
                {
                    Console.WriteLine($"  Image dimensions differ: {baseline.Width}x{baseline.Height} vs {actual.Width}x{actual.Height}");
                    return false;
                }

                int diffPixels = 0;
                int totalPixels = baseline.Width * baseline.Height;
                
                // Create diff image
                using var diff = new Image<Rgba32>(baseline.Width, baseline.Height);

                // Compare pixel by pixel using simple indexer
                for (int y = 0; y < baseline.Height; y++)
                {
                    for (int x = 0; x < baseline.Width; x++)
                    {
                        var baselinePixel = baseline[x, y];
                        var actualPixel = actual[x, y];

                        // Compare pixels with small tolerance for anti-aliasing
                        if (!PixelsAreSimilar(baselinePixel, actualPixel))
                        {
                            diffPixels++;
                            // Highlight difference in red
                            diff[x, y] = new Rgba32(255, 0, 0, 255);
                        }
                        else
                        {
                            // Show baseline pixel in grayscale with transparency
                            var gray = (byte)((baselinePixel.R + baselinePixel.G + baselinePixel.B) / 3);
                            diff[x, y] = new Rgba32(gray, gray, gray, 80);
                        }
                    }
                }

                var diffPercentage = (double)diffPixels / totalPixels * 100;

                Console.WriteLine($"  Pixel comparison: {diffPixels}/{totalPixels} pixels differ ({diffPercentage:F3}%)");
                Console.WriteLine($"  Tolerance: {tolerance}%");

                if (diffPercentage > tolerance)
                {
                    // Save diff image for investigation
                    var diffFile = actualFile.Replace("-actual.png", "-diff.png");
                    diff.Save(diffFile);
                    
                    Console.WriteLine($"  ? Images differ significantly!");
                    Console.WriteLine($"  Baseline: {baselineFile}");
                    Console.WriteLine($"  Actual:   {actualFile}");
                    Console.WriteLine($"  Diff:     {diffFile}");
                    return false;
                }

                Console.WriteLine($"  ? Images are similar (within tolerance)");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error comparing images: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Check if two pixels are similar (accounts for anti-aliasing)
        /// </summary>
        private bool PixelsAreSimilar(Rgba32 p1, Rgba32 p2, int threshold = 10)
        {
            // Allow small differences due to anti-aliasing, compression artifacts
            return Math.Abs(p1.R - p2.R) <= threshold &&
                   Math.Abs(p1.G - p2.G) <= threshold &&
                   Math.Abs(p1.B - p2.B) <= threshold &&
                   Math.Abs(p1.A - p2.A) <= threshold;
        }
    }
}
