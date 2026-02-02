using System.Threading.Tasks;
using Microsoft.Playwright;

namespace VSProject.Tests.Pages
{
    public class PlaywrightDevPage
    {
        private readonly IPage _page;
        private const string BaseUrl = "https://playwright.dev";

        private ILocator GetStartedLink => _page.Locator("text=Get Started");
        private ILocator GetStartedLinkByRole => _page.GetByRole(AriaRole.Link, new() { Name = "Get started" });
        private ILocator InstallationHeading => _page.GetByRole(AriaRole.Heading, new() { Name = "Installation" });

        public PlaywrightDevPage(IPage page)
        {
            _page = page;
        }

        public async Task NavigateAsync()
        {
            await _page.GotoAsync(BaseUrl);
        }

        public async Task<string> GetTitleAsync()
        {
            return await _page.TitleAsync();
        }

        public async Task<string?> GetGetStartedHrefAsync()
        {
            return await GetStartedLink.GetAttributeAsync("href");
        }

        public async Task ClickGetStartedAsync()
        {
            await GetStartedLink.ClickAsync();
        }

        public async Task ClickGetStartedByRoleAsync()
        {
            await GetStartedLinkByRole.ClickAsync();
        }

        public string GetCurrentUrl()
        {
            return _page.Url;
        }

        public ILocator GetInstallationHeading()
        {
            return InstallationHeading;
        }
    }
}
