Feature: 3. Accessibility Compliance
  As a user with disabilities
  I want the website to be accessible
  So that I can use all features without barriers

  @accessibility @wcag @critical
  Scenario: 1. Login page meets WCAG standards
    Given I am on the login page
    Then the page should have no accessibility violations

  @accessibility @wcag @critical
  Scenario: 2. Products page meets WCAG standards
    Given I am on the login page
    When I login with username standard_user and password secret_sauce
    Then the page should have no accessibility violations

  @accessibility @wcag @normal
  Scenario: 3. Shopping cart page meets WCAG standards
    Given I am logged in as "standard_user"
    When I add "Sauce Labs Backpack" to cart
    And I view my cart
    Then the page should have no accessibility violations

  @accessibility @wcag @normal
  Scenario: 4. Checkout page meets WCAG standards
    Given I am logged in as "standard_user"
    When I add "Sauce Labs Backpack" to cart
    And I view my cart
    And I proceed to checkout
    Then the page should have no accessibility violations

  @accessibility @forms @normal
  Scenario: 5. All form fields have proper labels
    Given I am on the login page
    Then all input fields should have accessible labels
    And all buttons should have accessible names

  @accessibility @keyboard @normal
  Scenario: 6. Keyboard navigation works correctly
    Given I am on the login page
    Then all interactive elements should be keyboard accessible
