using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deque.AxeCore.Commons;
using Deque.AxeCore.Playwright;
using Microsoft.Playwright;

namespace VSProject.Tests.Pages.Shopping
{
    /// <summary>
    /// Helper class for accessibility testing using Axe-core
    /// </summary>
    public class AccessibilityHelper
    {
        private readonly IPage _page;

        public AccessibilityHelper(IPage page)
        {
            _page = page;
        }

        /// <summary>
        /// Run accessibility scan on current page
        /// </summary>
        public async Task<AxeResult> ScanPageAsync(AxeRunOptions? options = null)
        {
            var axeResults = await _page.RunAxe(options);
            return axeResults;
        }

        /// <summary>
        /// Check if page has any accessibility violations
        /// </summary>
        public async Task<bool> HasViolationsAsync()
        {
            var results = await ScanPageAsync();
            return results.Violations.Any();
        }

        /// <summary>
        /// Get all accessibility violations
        /// </summary>
        public async Task<IEnumerable<AxeResultItem>> GetViolationsAsync()
        {
            var results = await ScanPageAsync();
            return results.Violations;
        }

        /// <summary>
        /// Get violations as formatted string for reporting
        /// </summary>
        public async Task<string> GetViolationsReportAsync()
        {
            var violations = await GetViolationsAsync();
            
            if (!violations.Any())
            {
                return "? No accessibility violations found";
            }

            var report = $"Found {violations.Count()} accessibility violations:\n\n";
            
            foreach (var violation in violations)
            {
                report += $"[{violation.Impact?.ToUpper()}] {violation.Help}\n";
                report += $"  Rule: {violation.Id}\n";
                report += $"  Description: {violation.Description}\n";
                report += $"  Help URL: {violation.HelpUrl}\n";
                report += $"  Affected elements: {violation.Nodes.Count()}\n";
                
                foreach (var node in violation.Nodes.Take(3)) // Show first 3 elements
                {
                    report += $"    - {node.Html}\n";
                    if (node.Target != null)
                    {
                        report += $"      Target: {string.Join(", ", node.Target)}\n";
                    }
                }
                
                if (violation.Nodes.Count() > 3)
                {
                    report += $"    ... and {violation.Nodes.Count() - 3} more\n";
                }
                
                report += "\n";
            }

            return report;
        }

        /// <summary>
        /// Check if all input fields have labels
        /// </summary>
        public async Task<bool> AllInputsHaveLabelsAsync()
        {
            var inputs = await _page.Locator("input[type='text'], input[type='password'], input[type='email']").AllAsync();
            
            foreach (var input in inputs)
            {
                var id = await input.GetAttributeAsync("id");
                var ariaLabel = await input.GetAttributeAsync("aria-label");
                var ariaLabelledBy = await input.GetAttributeAsync("aria-labelledby");
                var placeholder = await input.GetAttributeAsync("placeholder");
                
                // Check if input has associated label
                var hasLabel = !string.IsNullOrEmpty(id) && await _page.Locator($"label[for='{id}']").CountAsync() > 0;
                var hasAriaLabel = !string.IsNullOrEmpty(ariaLabel) || !string.IsNullOrEmpty(ariaLabelledBy);
                
                if (!hasLabel && !hasAriaLabel && string.IsNullOrEmpty(placeholder))
                {
                    return false;
                }
            }
            
            return true;
        }

        /// <summary>
        /// Check if all buttons have accessible names
        /// </summary>
        public async Task<bool> AllButtonsHaveAccessibleNamesAsync()
        {
            var buttons = await _page.Locator("button, input[type='submit'], input[type='button']").AllAsync();
            
            foreach (var button in buttons)
            {
                var text = await button.InnerTextAsync();
                var ariaLabel = await button.GetAttributeAsync("aria-label");
                var value = await button.GetAttributeAsync("value");
                
                if (string.IsNullOrWhiteSpace(text) && string.IsNullOrWhiteSpace(ariaLabel) && string.IsNullOrWhiteSpace(value))
                {
                    return false;
                }
            }
            
            return true;
        }

        /// <summary>
        /// Check if all interactive elements are keyboard accessible
        /// </summary>
        public async Task<bool> AllInteractiveElementsKeyboardAccessibleAsync()
        {
            var interactiveElements = await _page.Locator("a, button, input, select, textarea, [tabindex]").AllAsync();
            
            foreach (var element in interactiveElements)
            {
                var tabIndex = await element.GetAttributeAsync("tabindex");
                
                // tabindex="-1" means not keyboard accessible
                if (tabIndex == "-1")
                {
                    // Check if element is hidden or disabled
                    var isVisible = await element.IsVisibleAsync();
                    var isDisabled = await element.IsDisabledAsync();
                    
                    if (isVisible && !isDisabled)
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }

        /// <summary>
        /// Get violations by impact level (critical, serious, moderate, minor)
        /// </summary>
        public async Task<Dictionary<string, int>> GetViolationsByImpactAsync()
        {
            var violations = await GetViolationsAsync();
            
            return violations
                .GroupBy(v => v.Impact?.ToLower() ?? "unknown")
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}
