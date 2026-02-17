using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using VSProject.Tests.Pages.PlaywrightDev;

namespace VSProject.Tests.PlaywrightDev
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    [Category("PlaywrightDev")]
    public class PlaywrightDevTests : PlaywrightTestBase
    {
        private PlaywrightDevPage _playwrightPage;

        [SetUp]
        public void SetUp()
        {
            _playwrightPage = new PlaywrightDevPage(Page);
        }

        [Test]
        [Category("Smoke")]
        [Description("Verify homepage has Playwright in title and Get Started link points to intro page")]
        public async Task HomepageHasPlaywrightInTitleAndGetStartedLinkLinkingToTheIntroPage()
        {
            await _playwrightPage.NavigateAsync();

            await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));

            var href = await _playwrightPage.GetGetStartedHrefAsync();
            Assert.That(href, Is.EqualTo("/docs/intro"));

            await _playwrightPage.ClickGetStartedAsync();

            await Expect(Page).ToHaveURLAsync(new Regex(".*intro"));
        }

        [Test]
        [Category("Smoke")]
        [Description("Verify Get Started link navigates to Installation page")]
        public async Task GetStartedLink()
        {
            await _playwrightPage.NavigateAsync();

            await _playwrightPage.ClickGetStartedAsync();

            await Expect(_playwrightPage.GetInstallationHeading()).ToBeVisibleAsync();
        }
    }
}

