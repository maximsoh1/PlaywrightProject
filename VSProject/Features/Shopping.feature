Feature: 2. Complete Shopping Flow
As a customer
I want to browse products, add them to cart, and checkout
So that I can complete my purchase

Background:
  Given I am logged in as "standard_user"

# === ADD OPERATIONS ===
  
@functional @cart @normal
Scenario: 1. Add single product to cart
  When I add "Sauce Labs Backpack" to cart
  Then the cart should show badge "1"

@functional @cart @multiple-items @normal
Scenario: 2. Add multiple products to cart
  When I add the following products to cart:
    | Product                  |
    | Sauce Labs Backpack      |
    | Sauce Labs Bike Light    |
    | Sauce Labs Bolt T-Shirt  |
  And I view my cart
  Then the cart should contain 3 items

# === VIEW OPERATIONS ===

@functional @cart @view @normal
Scenario: 3. View cart with added products
  When I add "Sauce Labs Backpack" to cart
  And I view my cart
  Then the cart should contain 1 item
  And the cart should contain "Sauce Labs Backpack"

# === DELETE OPERATIONS ===

@functional @cart @delete @normal
Scenario: 4. Remove product from cart
  When I add "Sauce Labs Backpack" to cart
  And I view my cart
  And I remove "Sauce Labs Backpack" from cart
  Then the cart should be empty

# === E2E CRITICAL PATH ===

@smoke @e2e @critical-path @blocker
Scenario: 5. Complete shopping journey
  When I add "Sauce Labs Backpack" to cart
  And I view my cart
  Then the cart should contain 1 item
  And the cart should contain "Sauce Labs Backpack"
  When I proceed to checkout
  And I fill in customer information:
    | FirstName | LastName | PostalCode |
    | John      | Doe      | 12345      |
  And I complete the checkout
  Then the order should be confirmed

