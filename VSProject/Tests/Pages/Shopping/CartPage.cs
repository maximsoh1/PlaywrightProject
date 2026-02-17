using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace VSProject.Tests.Pages.Shopping
{
    /// <summary>
    /// Shopping cart page
    /// </summary>
    public class CartPage : BasePage
    {
        // Locators
        private ILocator PageTitle => Page.GetByText("Your Cart").First;
        private ILocator CheckoutButton => Page.GetByRole(AriaRole.Button, new() { Name = "Checkout" });
        private ILocator ContinueShoppingButton => Page.GetByRole(AriaRole.Button, new() { Name = "Continue Shopping" });
        private ILocator CartItems => Page.Locator(".cart_item");

        public CartPage(IPage page) : base(page)
        {
        }

        /// <summary>
        /// Check if on Cart page
        /// </summary>
        public async Task<bool> IsOnCartPageAsync()
        {
            var title = await PageTitle.InnerTextAsync();
            return title == "Your Cart";
        }

        /// <summary>
        /// Get cart items count
        /// </summary>
        public async Task<int> GetCartItemsCountAsync()
        {
            return await CartItems.CountAsync();
        }

        /// <summary>
        /// Get all product names in cart
        /// </summary>
        public async Task<List<string>> GetCartItemNamesAsync()
        {
            var items = await Page.Locator(".inventory_item_name").AllAsync();
            var names = new List<string>();
            
            foreach (var item in items)
            {
                names.Add(await item.InnerTextAsync());
            }
            
            return names;
        }

        /// <summary>
        /// Remove product from cart by name
        /// </summary>
        public async Task RemoveProductAsync(string productName)
        {
            var productItem = Page.Locator(".cart_item").Filter(new() { HasText = productName });
            var removeButton = productItem.GetByRole(AriaRole.Button, new() { Name = "Remove" });
            await removeButton.ClickAsync();
        }

        /// <summary>
        /// Click Checkout button
        /// </summary>
        public async Task ClickCheckoutAsync()
        {
            await CheckoutButton.ClickAsync();
        }

        /// <summary>
        /// Click Continue Shopping button
        /// </summary>
        public async Task ContinueShoppingAsync()
        {
            await ContinueShoppingButton.ClickAsync();
        }

        /// <summary>
        /// Check if product is in cart
        /// </summary>
        public async Task<bool> IsProductInCartAsync(string productName)
        {
            var names = await GetCartItemNamesAsync();
            return names.Contains(productName);
        }
    }
}

