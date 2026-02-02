using System.Threading.Tasks;
using Microsoft.Playwright;

namespace VSProject.Tests.Pages
{
    public class ContactUsPage
    {
        private readonly IPage _page;
        private const string BaseUrl = "https://amdaris.com/#pardot-form";

        private IFrameLocator PardotFrame => _page.Locator("#par-form").ContentFrame;
        private ILocator FirstNameField => PardotFrame.GetByRole(AriaRole.Textbox, new() { Name = "First Name*" });
        private ILocator LastNameField => PardotFrame.GetByRole(AriaRole.Textbox, new() { Name = "Last Name*" });
        private ILocator CompanyField => PardotFrame.GetByRole(AriaRole.Textbox, new() { Name = "Company*" });
        private ILocator JobTitleField => PardotFrame.GetByRole(AriaRole.Textbox, new() { Name = "Job Title*" });
        private ILocator PhoneField => PardotFrame.GetByRole(AriaRole.Textbox, new() { Name = "Business Phone Number*" });
        private ILocator EmailField => PardotFrame.GetByRole(AriaRole.Textbox, new() { Name = "Business Email Address*" });
        private ILocator MessageField => PardotFrame.GetByRole(AriaRole.Textbox, new() { Name = "Tell us what you're looking" });
        private ILocator HelpDropdown => PardotFrame.GetByLabel("What can we help with?");
        private ILocator SubmitButton => PardotFrame.GetByRole(AriaRole.Button, new() { Name = "Submit" });
        private ILocator RequiredFieldError => PardotFrame.GetByText("This field is required.");

        public ContactUsPage(IPage page)
        {
            _page = page;
        }

        public async Task NavigateAsync()
        {
            await _page.GotoAsync(BaseUrl);
        }

        public async Task FillFirstNameAsync(string firstName)
        {
            await FirstNameField.ClickAsync();
            await FirstNameField.FillAsync(firstName);
        }

        public async Task FillLastNameAsync(string lastName)
        {
            await LastNameField.ClickAsync();
            await LastNameField.FillAsync(lastName);
        }

        public async Task FillCompanyAsync(string company)
        {
            await CompanyField.ClickAsync();
            await CompanyField.FillAsync(company);
        }

        public async Task FillJobTitleAsync(string jobTitle)
        {
            await JobTitleField.ClickAsync();
            await JobTitleField.FillAsync(jobTitle);
        }

        public async Task FillPhoneAsync(string phone)
        {
            await PhoneField.ClickAsync();
            await PhoneField.FillAsync(phone);
        }

        public async Task FillEmailAsync(string email)
        {
            await EmailField.ClickAsync();
            await EmailField.FillAsync(email);
        }

        public async Task FillMessageAsync(string message)
        {
            await MessageField.ClickAsync();
            await MessageField.FillAsync(message);
        }

        public async Task SelectHelpOptionAsync(string value)
        {
            await HelpDropdown.SelectOptionAsync(new[] { value });
        }

        public async Task ClickSubmitAsync()
        {
            await SubmitButton.ClickAsync();
        }

        public async Task SubmitEmptyFormAsync()
        {
            await ClickSubmitAsync();
        }

        public ILocator GetRequiredFieldError(int index = 0)
        {
            return index == 0 ? RequiredFieldError.First : RequiredFieldError.Nth(index);
        }

        public async Task<bool> IsFirstNameVisibleAsync()
        {
            return await FirstNameField.IsVisibleAsync();
        }

        public async Task<bool> IsLastNameVisibleAsync()
        {
            return await LastNameField.IsVisibleAsync();
        }

        public async Task<bool> IsCompanyVisibleAsync()
        {
            return await CompanyField.IsVisibleAsync();
        }

        public async Task<bool> IsJobTitleVisibleAsync()
        {
            return await JobTitleField.IsVisibleAsync();
        }

        public async Task FillFormPartiallyAsync(string firstName, string jobTitle, string helpOption)
        {
            await FillFirstNameAsync(firstName);
            await SelectHelpOptionAsync(helpOption);
            await FillJobTitleAsync(jobTitle);
        }
    }
}
