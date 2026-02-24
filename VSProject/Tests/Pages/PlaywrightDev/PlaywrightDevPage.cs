using System.Threading.Tasks;
using Microsoft.Playwright;

namespace VSProject.Tests.Pages.PlaywrightDev
{
    /// <summary>
    /// Playwright.dev website page
    /// </summary>
    public class PlaywrightDevPage : BasePage
    {
        private const string BaseUrl = "https://playwright.dev";

        // Locators
        private ILocator GetStartedLinkByRole => Page.GetByRole(AriaRole.Link, new() { Name = "Get started" });
        private ILocator InstallationHeading => Page.GetByRole(AriaRole.Heading, new() { Name = "Installation" });

        public PlaywrightDevPage(IPage page) : base(page)
        {
        }

        public async Task NavigateAsync()
        {
            await Page.GotoAsync(BaseUrl);
        }

        public async Task<string?> GetGetStartedHrefAsync()
        {
            return await GetStartedLinkByRole.GetAttributeAsync("href");
        }

        public async Task ClickGetStartedAsync()
        {
            await GetStartedLinkByRole.ClickAsync();
        }

        public ILocator GetInstallationHeading()
        {
            return InstallationHeading;
        }
    }
}
