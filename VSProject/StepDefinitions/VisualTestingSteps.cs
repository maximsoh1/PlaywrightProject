using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;
using VSProject.Tests.Pages.Shopping;

namespace VSProject.StepDefinitions
{
    /// <summary>
    /// Step definitions for Visual Testing feature
    /// </summary>
    [Binding]
    public class VisualTestingSteps
    {
        private readonly IPage _page;
        private VisualTestingHelper _visualHelper;
        private LoginPage _loginPage;
        private ProductsPage _productsPage;

        public VisualTestingSteps(IPage page)
        {
            _page = page;
            _visualHelper = new VisualTestingHelper(_page);
            _loginPage = new LoginPage(_page);
            _productsPage = new ProductsPage(_page);
        }

        // === VIEWPORT SETUP ===

        [Given(@"I am on the login page with mobile viewport")]
        public async Task GivenIAmOnTheLoginPageWithMobileViewport()
        {
            await _visualHelper.SetMobileViewportAsync();
            await _loginPage.NavigateAsync();
        }

        [Given(@"I am on the login page with tablet viewport")]
        public async Task GivenIAmOnTheLoginPageWithTabletViewport()
        {
            await _visualHelper.SetTabletViewportAsync();
            await _loginPage.NavigateAsync();
        }

        // === PAGE SCREENSHOTS ===

        [Then(@"the login page should match the baseline screenshot")]
        public async Task ThenTheLoginPageShouldMatchTheBaselineScreenshot()
        {
            // Wait for page to be fully loaded
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            var result = await _visualHelper.ComparePageScreenshotAsync("login-page");
            
            if (!result)
            {
                var baselinePath = _visualHelper.GetBaselinePath();
                TestContext.WriteLine($"Screenshot saved to: {baselinePath}");
                TestContext.WriteLine("If this is the first run, baseline was created.");
                TestContext.WriteLine("Run test again to compare with baseline.");
            }
            
            Assert.That(result, Is.True, "Login page screenshot should match baseline");
        }

        [Then(@"the products page should match the baseline screenshot")]
        public async Task ThenTheProductsPageShouldMatchTheBaselineScreenshot()
        {
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            // Mask dynamic elements (like sort dropdown, prices might change)
            var result = await _visualHelper.ComparePageWithMaskAsync(
                "products-page",
                new[] { ".shopping_cart_badge" } // Mask cart badge as it's dynamic
            );
            
            Assert.That(result, Is.True, "Products page screenshot should match baseline");
        }

        [Then(@"the cart page should match the baseline screenshot")]
        public async Task ThenTheCartPageShouldMatchTheBaselineScreenshot()
        {
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            var result = await _visualHelper.ComparePageScreenshotAsync("cart-page");
            Assert.That(result, Is.True, "Cart page screenshot should match baseline");
        }

        [Then(@"the mobile login page should match the baseline screenshot")]
        public async Task ThenTheMobileLoginPageShouldMatchTheBaselineScreenshot()
        {
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            var result = await _visualHelper.ComparePageScreenshotAsync("login-page-mobile");
            Assert.That(result, Is.True, "Mobile login page screenshot should match baseline");
        }

        [Then(@"the tablet products page should match the baseline screenshot")]
        public async Task ThenTheTabletProductsPageShouldMatchTheBaselineScreenshot()
        {
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            var result = await _visualHelper.ComparePageScreenshotAsync("products-page-tablet");
            Assert.That(result, Is.True, "Tablet products page screenshot should match baseline");
        }

        // === ELEMENT SCREENSHOTS ===

        [Then(@"the first product card should match the baseline screenshot")]
        public async Task ThenTheFirstProductCardShouldMatchTheBaselineScreenshot()
        {
            var firstProduct = _page.Locator(".inventory_item").First;
            await firstProduct.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            
            var result = await _visualHelper.CompareElementScreenshotAsync(
                firstProduct, 
                "first-product-card"
            );
            
            Assert.That(result, Is.True, "First product card screenshot should match baseline");
        }

        [Then(@"the page header should match the baseline screenshot")]
        public async Task ThenThePageHeaderShouldMatchTheBaselineScreenshot()
        {
            var header = _page.Locator(".header_container");
            await header.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            
            var result = await _visualHelper.CompareElementScreenshotAsync(header, "page-header");
            Assert.That(result, Is.True, "Page header screenshot should match baseline");
        }

        [Then(@"the shopping cart icon should match the baseline screenshot")]
        public async Task ThenTheShoppingCartIconShouldMatchTheBaselineScreenshot()
        {
            var cartIcon = _page.Locator(".shopping_cart_link");
            await cartIcon.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            
            var result = await _visualHelper.CompareElementScreenshotAsync(cartIcon, "cart-icon");
            Assert.That(result, Is.True, "Shopping cart icon screenshot should match baseline");
        }

        // === INTERACTIVE STATES ===

        [When(@"I hover over the login button")]
        public async Task WhenIHoverOverTheLoginButton()
        {
            var loginButton = _page.Locator("#login-button");
            await loginButton.HoverAsync();
            await _page.WaitForTimeoutAsync(500); // Wait for hover effect
        }

        [Then(@"the login button should match the baseline screenshot")]
        public async Task ThenTheLoginButtonShouldMatchTheBaselineScreenshot()
        {
            var loginButton = _page.Locator("#login-button");
            
            var result = await _visualHelper.CompareElementHoverStateAsync(
                loginButton, 
                "login-button"
            );
            
            Assert.That(result, Is.True, "Login button screenshot should match baseline");
        }
    }
}
