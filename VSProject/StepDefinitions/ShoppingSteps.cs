using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;
using Reqnroll.Assist;
using VSProject.Tests.Pages.Shopping;

namespace VSProject.StepDefinitions
{
    /// <summary>
    /// Step definitions for Shopping feature
    /// </summary>
    [Binding]
    public class ShoppingSteps
    {
        private readonly IPage _page;
        private LoginPage _loginPage;
        private ProductsPage _productsPage;
        private CartPage _cartPage;
        private CheckoutPage _checkoutPage;

        public ShoppingSteps(IPage page)
        {
            _page = page;
            _loginPage = new LoginPage(_page);
            _productsPage = new ProductsPage(_page);
            _cartPage = new CartPage(_page);
            _checkoutPage = new CheckoutPage(_page);
        }

        [Given(@"I am logged in as ""(.*)""")]
        public async Task GivenIAmLoggedInAs(string username)
        {
            await _loginPage.NavigateAsync();
            await _loginPage.LoginAsync(username, "secret_sauce");
        }

        [When(@"I add ""(.*)"" to cart")]
        public async Task WhenIAddToCart(string productName)
        {
            await _productsPage.AddProductToCartAsync(productName);
        }

        [When(@"I add the following products to cart:")]
        public async Task WhenIAddTheFollowingProductsToCart(Table table)
        {
            foreach (var row in table.Rows)
            {
                var productName = row["Product"];
                await _productsPage.AddProductToCartAsync(productName);
            }
        }

        [When(@"I view my cart")]
        public async Task WhenIViewMyCart()
        {
            await _productsPage.OpenCartAsync();
        }

        [When(@"I remove ""(.*)"" from cart")]
        public async Task WhenIRemoveFromCart(string productName)
        {
            await _cartPage.RemoveProductAsync(productName);
        }

        [When(@"I proceed to checkout")]
        public async Task WhenIProceedToCheckout()
        {
            await _cartPage.ClickCheckoutAsync();
        }

        [When(@"I fill in customer information:")]
        public async Task WhenIFillInCustomerInformation(Table table)
        {
            var data = table.CreateInstance<CustomerInfo>();
            await _checkoutPage.FillCustomerInfoAsync(data.FirstName, data.LastName, data.PostalCode);
        }

        [When(@"I complete the checkout")]
        public async Task WhenICompleteTheCheckout()
        {
            await _checkoutPage.ClickContinueAsync();
            await _checkoutPage.ClickFinishAsync();
        }

        [Then(@"the cart should contain (.*) item")]
        [Then(@"the cart should contain (.*) items")]
        public async Task ThenTheCartShouldContainItems(int expectedCount)
        {
            var actualCount = await _cartPage.GetCartItemsCountAsync();
            Assert.That(actualCount, Is.EqualTo(expectedCount), 
                $"Expected {expectedCount} items in cart, but found {actualCount}");
        }

        [Then(@"the cart should contain ""(.*)""")]
        public async Task ThenTheCartShouldContain(string productName)
        {
            var hasProduct = await _cartPage.IsProductInCartAsync(productName);
            Assert.That(hasProduct, Is.True, $"Cart should contain '{productName}'");
        }

        [Then(@"the cart should be empty")]
        public async Task ThenTheCartShouldBeEmpty()
        {
            var count = await _cartPage.GetCartItemsCountAsync();
            Assert.That(count, Is.EqualTo(0), "Cart should be empty");
        }

        [Then(@"the order should be confirmed")]
        public async Task ThenTheOrderShouldBeConfirmed()
        {
            var isComplete = await _checkoutPage.IsOrderCompleteAsync();
            Assert.That(isComplete, Is.True, "Order should be confirmed");
        }

        [Then(@"the cart should show badge ""(.*)""")]
        public async Task ThenTheCartShouldShowBadge(string expectedBadgeValue)
        {
            var cartCount = await _productsPage.GetCartItemsCountAsync();
            Assert.That(cartCount.ToString(), Is.EqualTo(expectedBadgeValue), 
                $"Cart badge should show '{expectedBadgeValue}'");
        }

        // Helper class for table mapping
        public class CustomerInfo
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string PostalCode { get; set; }
        }
    }
}
