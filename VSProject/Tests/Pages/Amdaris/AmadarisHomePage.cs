using System.Threading.Tasks;
using Microsoft.Playwright;

namespace VSProject.Tests.Pages.Amdaris
{
    /// <summary>
    /// Amdaris homepage
    /// </summary>
    public class AmadarisHomePage : BasePage
    {
        private const string BaseUrl = "https://amdaris.com/";

        // Locators
        private ILocator GetInTouchLink => Page.GetByText("Get in touch", new() { Exact = true });

        public AmadarisHomePage(IPage page) : base(page)
        {
        }

        /// <summary>
        /// Navigate to homepage
        /// </summary>
        public async Task NavigateAsync()
        {
            await Page.GotoAsync(BaseUrl, new() { WaitUntil = WaitUntilState.NetworkIdle });
        }

        /// <summary>
        /// Navigate to contact form
        /// </summary>
        public async Task NavigateToContactFormAsync()
        {
            await Page.GotoAsync($"{BaseUrl}#pardot-form", new() { WaitUntil = WaitUntilState.NetworkIdle });
        }

        /// <summary>
        /// Get "Get in touch" link text
        /// </summary>
        public async Task<string> GetInTouchTextAsync()
        {
            await GetInTouchLink.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });
            return await GetInTouchLink.InnerTextAsync();
        }

        /// <summary>
        /// Check if "Get in touch" link is visible
        /// </summary>
        public async Task<bool> IsGetInTouchVisibleAsync()
        {
            try
            {
                await GetInTouchLink.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });
                return await GetInTouchLink.IsVisibleAsync();
            }
            catch
            {
                return false;
            }
        }
    }
}


