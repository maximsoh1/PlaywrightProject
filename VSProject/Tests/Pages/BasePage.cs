using System.Threading.Tasks;
using Microsoft.Playwright;

namespace VSProject.Tests.Pages
{
    /// <summary>
    /// Base class for all Page Objects
    /// </summary>
    public abstract class BasePage
    {
        /// <summary>
        /// Playwright page instance
        /// </summary>
        protected readonly IPage Page;

        /// <summary>
        /// Constructor
        /// </summary>
        protected BasePage(IPage page)
        {
            Page = page;
        }

        /// <summary>
        /// Get current page URL
        /// </summary>
        public string GetCurrentUrl() => Page.Url;

        /// <summary>
        /// Get page title
        /// </summary>
        public async Task<string> GetTitleAsync() => await Page.TitleAsync();

        /// <summary>
        /// Take full-page screenshot
        /// </summary>
        public async Task TakeScreenshotAsync(string path)
        {
            await Page.ScreenshotAsync(new() { Path = path, FullPage = true });
        }
    }
}


