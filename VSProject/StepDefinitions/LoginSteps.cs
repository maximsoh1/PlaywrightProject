using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;
using VSProject.Tests.Pages.Shopping;

namespace VSProject.StepDefinitions
{
    /// <summary>
    /// Step definitions for Login feature
    /// </summary>
    [Binding]
    public class LoginSteps
    {
        private readonly IPage _page;
        private LoginPage _loginPage;
        private ProductsPage _productsPage;

        // Dependency Injection: Reqnroll provides IPage
        public LoginSteps(IPage page)
        {
            _page = page;
            _loginPage = new LoginPage(_page);
            _productsPage = new ProductsPage(_page);
        }

        [Given(@"I am on the login page")]
        public async Task GivenIAmOnTheLoginPage()
        {
            await _loginPage.NavigateAsync();
        }

        [When(@"I login with username (.*) and password (.*)")]
        public async Task WhenILoginWithUsernameAndPassword(string username, string password)
        {
            await _loginPage.LoginAsync(username, password);
        }

        [Then(@"I should be redirected to the products page")]
        public async Task ThenIShouldBeRedirectedToTheProductsPage()
        {
            var isOnProductsPage = await _productsPage.IsOnProductsPageAsync();
            Assert.That(isOnProductsPage, Is.True, "User should be on Products page after login");
        }

        [Then(@"I should see an error message ""(.*)""")]
        public async Task ThenIShouldSeeAnErrorMessage(string expectedMessage)
        {
            var isErrorVisible = await _loginPage.IsErrorMessageVisibleAsync();
            Assert.That(isErrorVisible, Is.True, "Error message should be visible");

            var actualMessage = await _loginPage.GetErrorMessageTextAsync();
            Assert.That(actualMessage, Does.Contain(expectedMessage), 
                $"Expected error message to contain '{expectedMessage}'");
        }
    }
}
