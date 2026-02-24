Feature: 4. Visual Regression Testing
  As a QA engineer
  I want to verify visual appearance of pages
  So that I can detect unintended UI changes

  @visual @smoke @critical
  Scenario: 1. Login page visual appearance
    Given I am on the login page
    Then the login page should match the baseline screenshot

  @visual @smoke @critical
  Scenario: 2. Products page visual appearance
    Given I am on the login page
    When I login with username standard_user and password secret_sauce
    Then the products page should match the baseline screenshot

  @visual @components @normal
  Scenario: 3. Product card component appearance
    Given I am on the login page
    When I login with username standard_user and password secret_sauce
    Then the first product card should match the baseline screenshot

  @visual @cart @normal
  Scenario: 4. Shopping cart visual appearance
    Given I am logged in as "standard_user"
    When I add "Sauce Labs Backpack" to cart
    And I view my cart
    Then the cart page should match the baseline screenshot

  @visual @responsive @normal
  Scenario: 5. Login page on mobile viewport
    Given I am on the login page with mobile viewport
    Then the mobile login page should match the baseline screenshot

  @visual @responsive @normal
  Scenario: 6. Products page on tablet viewport
    Given I am on the login page with tablet viewport
    When I login with username standard_user and password secret_sauce
    Then the tablet products page should match the baseline screenshot

  @visual @elements @normal
  Scenario: 7. Header and footer elements
    Given I am on the login page
    When I login with username standard_user and password secret_sauce
    Then the page header should match the baseline screenshot
    And the shopping cart icon should match the baseline screenshot

  @visual @states @normal
  Scenario: 8. Button hover states
    Given I am on the login page
    When I hover over the login button
    Then the login button should match the baseline screenshot
