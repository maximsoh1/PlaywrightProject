using System.Threading.Tasks;
using Microsoft.Playwright;

namespace VSProject.Tests.Pages.Shopping
{
    /// <summary>
    /// Products catalog page
    /// </summary>
    public class ProductsPage : BasePage
    {
        // Locators
        private ILocator PageTitle => Page.Locator(".title");
        private ILocator ShoppingCartLink => Page.Locator(".shopping_cart_link");
        private ILocator CartBadge => Page.Locator(".shopping_cart_badge");
        private ILocator SortDropdown => Page.Locator("[data-test='product-sort-container']");

        public ProductsPage(IPage page) : base(page)
        {
        }

        /// <summary>
        /// Check if on Products page
        /// </summary>
        public async Task<bool> IsOnProductsPageAsync()
        {
            try
            {
                await PageTitle.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });
                var title = await PageTitle.InnerTextAsync();
                return title == "Products";
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Add product to cart by name
        /// </summary>
        public async Task AddProductToCartAsync(string productName)
        {
            var productItem = Page.Locator(".inventory_item").Filter(new() { HasText = productName });
            var addButton = productItem.GetByRole(AriaRole.Button, new() { Name = "Add to cart" });
            await addButton.ClickAsync();
        }

        /// <summary>
        /// Get cart items count from badge
        /// </summary>
        public async Task<int> GetCartItemsCountAsync()
        {
            try
            {
                var countText = await CartBadge.InnerTextAsync();
                return int.Parse(countText);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Open shopping cart
        /// </summary>
        public async Task OpenCartAsync()
        {
            await ShoppingCartLink.ClickAsync();
        }

        /// <summary>
        /// Sort products (az, za, lohi, hilo)
        /// </summary>
        public async Task SortProductsAsync(string sortOption)
        {
            await SortDropdown.SelectOptionAsync(sortOption);
        }

        /// <summary>
        /// Get first product name
        /// </summary>
        public async Task<string> GetFirstProductNameAsync()
        {
            var firstProduct = Page.Locator(".inventory_item_name").First;
            return await firstProduct.InnerTextAsync();
        }
    }
}

