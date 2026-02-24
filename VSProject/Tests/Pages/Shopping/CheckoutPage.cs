using System.Threading.Tasks;
using Microsoft.Playwright;

namespace VSProject.Tests.Pages.Shopping
{
    /// <summary>
    /// Checkout page
    /// </summary>
    public class CheckoutPage : BasePage
    {
        // Locators for Step 1: Customer Information
        private ILocator FirstNameField => Page.Locator("#first-name");
        private ILocator LastNameField => Page.Locator("#last-name");
        private ILocator PostalCodeField => Page.Locator("#postal-code");
        private ILocator ContinueButton => Page.Locator("#continue");
        private ILocator CancelButton => Page.Locator("#cancel");

        // Locators for Step 2: Overview
        private ILocator FinishButton => Page.Locator("#finish");
        private ILocator TotalPrice => Page.Locator(".summary_total_label");

        // Locators for Step 3: Complete
        private ILocator CompleteHeader => Page.Locator(".complete-header");
        private ILocator BackHomeButton => Page.Locator("#back-to-products");

        public CheckoutPage(IPage page) : base(page)
        {
        }

        /// <summary>
        /// Fill customer information
        /// </summary>
        public async Task FillCustomerInfoAsync(string firstName, string lastName, string postalCode)
        {
            await FirstNameField.FillAsync(firstName);
            await LastNameField.FillAsync(lastName);
            await PostalCodeField.FillAsync(postalCode);
        }

        /// <summary>
        /// Click Continue button
        /// </summary>
        public async Task ClickContinueAsync()
        {
            await ContinueButton.ClickAsync();
        }

        /// <summary>
        /// Get total price
        /// </summary>
        public async Task<string> GetTotalPriceAsync()
        {
            return await TotalPrice.InnerTextAsync();
        }

        /// <summary>
        /// Click Finish button
        /// </summary>
        public async Task ClickFinishAsync()
        {
            await FinishButton.ClickAsync();
        }

        /// <summary>
        /// Check if order completed successfully
        /// </summary>
        public async Task<bool> IsOrderCompleteAsync()
        {
            var header = await CompleteHeader.InnerTextAsync();
            return header.Contains("Thank you for your order");
        }

        /// <summary>
        /// Go back to homepage
        /// </summary>
        public async Task BackToHomeAsync()
        {
            await BackHomeButton.ClickAsync();
        }
    }
}

