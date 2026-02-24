using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using VSProject.Tests.Pages.Amdaris;
using VSProject.Tests.PlaywrightDev;

namespace VSProject.Tests.Amdaris
{
    [TestFixture]
    [Category("ContactUs")]
    public class ContactUsTest : PlaywrightTestBase
    {
        private ContactUsPage _contactUsPage;

        [SetUp]
        public void SetUp()
        {
            _contactUsPage = new ContactUsPage(Page);
        }

        [Test]
        [Category("Validation")]
        [Description("Verify that required field validation messages are displayed when form is submitted empty")]
        public async Task VerifyRequiredFieldValidationMessages()
        {
            await _contactUsPage.NavigateAsync();
            
            await _contactUsPage.SubmitEmptyFormAsync();

            await Expect(_contactUsPage.GetRequiredFieldError(0)).ToBeVisibleAsync();
            await Expect(_contactUsPage.GetRequiredFieldError(1)).ToBeVisibleAsync();
            await Expect(_contactUsPage.GetRequiredFieldError(2)).ToBeVisibleAsync();
        }

        [Test]
        [Category("Validation")]
        [Description("Verify all required fields are visible on the form")]
        public async Task VerifyAllRequiredFieldsAreVisible()
        {
            await _contactUsPage.NavigateAsync();

            var isFirstNameVisible = await _contactUsPage.IsFirstNameVisibleAsync();
            var isLastNameVisible = await _contactUsPage.IsLastNameVisibleAsync();
            var isCompanyVisible = await _contactUsPage.IsCompanyVisibleAsync();
            var isJobTitleVisible = await _contactUsPage.IsJobTitleVisibleAsync();

            Assert.That(isFirstNameVisible, Is.True, "First Name field should be visible");
            Assert.That(isLastNameVisible, Is.True, "Last Name field should be visible");
            Assert.That(isCompanyVisible, Is.True, "Company field should be visible");
            Assert.That(isJobTitleVisible, Is.True, "Job Title field should be visible");
        }

        [Test]
        [Category("Functional")]
        [Description("Verify user can fill form fields with valid data")]
        public async Task VerifyUserCanFillFormFields()
        {
            await _contactUsPage.NavigateAsync();

            await _contactUsPage.FillFormPartiallyAsync(
                firstName: "Maxim",
                jobTitle: "QA Engineer",
                helpOption: "2144232"
            );

            Assert.Pass("Form fields filled successfully");
        }
    }
}