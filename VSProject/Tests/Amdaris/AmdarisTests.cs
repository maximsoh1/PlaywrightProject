using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using VSProject.Tests.Pages.Amdaris;
using VSProject.Tests.PlaywrightDev;

namespace VSProject.Tests.Amdaris
{
    [TestFixture]
    [Category("Amdaris")]
    public class AmadarisTests : PlaywrightTestBase
    {
        private AmadarisHomePage _homePage;

        [SetUp]
        public void SetUp()
        {
            _homePage = new AmadarisHomePage(Page);
        }

        [Test]
        [Category("Smoke")]
        [Description("Verify Get in touch link displays correct text")]
        public async Task VerifyGetInTouchTextIsDisplayed()
        {
            await _homePage.NavigateAsync();
            var text = await _homePage.GetInTouchTextAsync();
            Assert.That(text, Is.EqualTo("Get in touch"), "Get in touch link should display correct text");
        }

        [Test]
        [Category("Smoke")]
        [Description("Verify Get in touch link is visible on homepage")]
        public async Task VerifyGetInTouchLinkIsVisible()
        {
            await _homePage.NavigateAsync();
            var isVisible = await _homePage.IsGetInTouchVisibleAsync();
            Assert.That(isVisible, Is.True, "Get in touch link should be visible");
        }
    }
}
