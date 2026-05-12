# E-Commerce API Unit Tests

This test project contains comprehensive unit tests for the E-Commerce API backend services by Elena Voican.

## Test Coverage

### AuthServiceTests (11 tests)
- User registration with valid input
- Registration with existing user
- User login with valid credentials
- Login with invalid email
- Login with invalid password
- Input validation (empty/null email)

### OrderServiceTests (6 tests)
- Order creation with valid input
- Order creation with invalid shipping address
- Order creation with empty cart
- Order creation with invalid cart items
- Retrieve order by ID
- Retrieve all orders for a user

### CartServiceTests (7 tests)
- Calculate cart total with multiple items
- Calculate total skipping non-existent products
- Calculate total for empty cart
- Validate cart items with sufficient stock
- Validate cart with insufficient stock
- Validate cart with non-existent products

### ProductServiceTests (8 tests)
- Retrieve all products
- Handle empty product database
- Retrieve single product by ID
- Handle non-existent product ID
- Search products by keyword
- Case-insensitive search
- Handle zero stock products
