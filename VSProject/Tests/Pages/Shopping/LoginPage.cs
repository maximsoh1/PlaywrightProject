using System.Threading.Tasks;
using Microsoft.Playwright;

namespace VSProject.Tests.Pages.Shopping
{
    /// <summary>
    /// Login page for saucedemo.com
    /// Valid credentials: standard_user / secret_sauce
    /// </summary>
    public class LoginPage : BasePage
    {
        private const string BaseUrl = "https://www.saucedemo.com/";

        // Locators
        private ILocator UsernameField => Page.Locator("#user-name");
        private ILocator PasswordField => Page.Locator("#password");
        private ILocator LoginButton => Page.Locator("#login-button");
        private ILocator ErrorMessage => Page.Locator("[data-test='error']");

        public LoginPage(IPage page) : base(page)
        {
        }

        /// <summary>
        /// Navigate to login page
        /// </summary>
        public async Task NavigateAsync()
        {
            await Page.GotoAsync(BaseUrl);
        }

        /// <summary>
        /// Login with credentials
        /// </summary>
        public async Task LoginAsync(string username, string password)
        {
            await UsernameField.FillAsync(username);
            await PasswordField.FillAsync(password);
            await LoginButton.ClickAsync();
        }

        /// <summary>
        /// Check if error message is visible
        /// </summary>
        public async Task<bool> IsErrorMessageVisibleAsync()
        {
            return await ErrorMessage.IsVisibleAsync();
        }

        /// <summary>
        /// Get error message text
        /// </summary>
        public async Task<string> GetErrorMessageTextAsync()
        {
            return await ErrorMessage.InnerTextAsync();
        }
    }
}

