using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;
using VSProject.Tests.Pages.Shopping;

namespace VSProject.StepDefinitions
{
    /// <summary>
    /// Step definitions for Accessibility feature
    /// </summary>
    [Binding]
    public class AccessibilitySteps
    {
        private readonly IPage _page;
        private AccessibilityHelper _accessibilityHelper;

        public AccessibilitySteps(IPage page)
        {
            _page = page;
            _accessibilityHelper = new AccessibilityHelper(_page);
        }

        [Then(@"the page should have no accessibility violations")]
        public async Task ThenThePageShouldHaveNoAccessibilityViolations()
        {
            var hasViolations = await _accessibilityHelper.HasViolationsAsync();
            
            if (hasViolations)
            {
                var report = await _accessibilityHelper.GetViolationsReportAsync();
                var violationsByImpact = await _accessibilityHelper.GetViolationsByImpactAsync();
                
                TestContext.WriteLine("\n=== ACCESSIBILITY VIOLATIONS ===");
                TestContext.WriteLine(report);
                TestContext.WriteLine("\n=== VIOLATIONS BY IMPACT ===");
                foreach (var impact in violationsByImpact)
                {
                    TestContext.WriteLine($"{impact.Key}: {impact.Value}");
                }
                
                Assert.Fail($"Page has accessibility violations. See test output for details.");
            }
            
            TestContext.WriteLine("? No accessibility violations found");
        }

        [Then(@"all input fields should have accessible labels")]
        public async Task ThenAllInputFieldsShouldHaveAccessibleLabels()
        {
            var allHaveLabels = await _accessibilityHelper.AllInputsHaveLabelsAsync();
            Assert.That(allHaveLabels, Is.True, "All input fields should have accessible labels");
        }

        [Then(@"all buttons should have accessible names")]
        public async Task ThenAllButtonsShouldHaveAccessibleNames()
        {
            var allHaveNames = await _accessibilityHelper.AllButtonsHaveAccessibleNamesAsync();
            Assert.That(allHaveNames, Is.True, "All buttons should have accessible names");
        }

        [Then(@"all interactive elements should be keyboard accessible")]
        public async Task ThenAllInteractiveElementsShouldBeKeyboardAccessible()
        {
            var allAccessible = await _accessibilityHelper.AllInteractiveElementsKeyboardAccessibleAsync();
            Assert.That(allAccessible, Is.True, "All interactive elements should be keyboard accessible");
        }
    }
}
