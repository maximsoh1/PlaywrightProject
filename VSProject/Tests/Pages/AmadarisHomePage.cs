using System.Threading.Tasks;
using Microsoft.Playwright;

namespace VSProject.Tests.Pages
{
    public class AmadarisHomePage
    {
        private readonly IPage _page;
        private const string BaseUrl = "https://amdaris.com/";

        private ILocator GetInTouchLink => _page.GetByText("Get in touch", new() { Exact = true });

        public AmadarisHomePage(IPage page)
        {
            _page = page;
        }

        public async Task NavigateAsync()
        {
            await _page.GotoAsync(BaseUrl, new() { WaitUntil = WaitUntilState.NetworkIdle });
        }

        public async Task NavigateToContactFormAsync()
        {
            await _page.GotoAsync($"{BaseUrl}#pardot-form", new() { WaitUntil = WaitUntilState.NetworkIdle });
        }

        public async Task<string> GetInTouchTextAsync()
        {
            await GetInTouchLink.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });
            return await GetInTouchLink.InnerTextAsync();
        }

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

