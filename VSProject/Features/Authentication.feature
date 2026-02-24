Feature: 1. Authentication
As a user
I want to login to the shopping website
So that I can access my account and shop

@smoke @authentication @critical
@allure.owner:Maxim
Scenario: 1. Successful login with valid credentials
  Given I am on the login page
  When I login with username standard_user and password secret_sauce
  Then I should be redirected to the products page

@negative @security @authentication @critical
Scenario: 2. Login with invalid credentials
  Given I am on the login page
  When I login with username standard_user and password wrong_password
  Then I should see an error message "Username and password do not match"
