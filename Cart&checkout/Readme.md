# In-Memory Cart API (.NET 7)

This project implements a simple in-memory shopping cart feature as an ASP.NET Core Web API using .NET 7. It allows users (simulated via a mock user ID) to add items to a cart, view the cart, remove items, and perform a mock checkout.

## Features

*   Add items to the cart (Product ID, Quantity).
*   Remove items from the cart (by Product ID, optionally specifying quantity to remove).
*   View the current contents of the cart.
*   Perform a mock checkout, which calculates the total cost, returns an order summary, and clears the cart.
*   In-memory storage (no database required).
*   Uses a mock product catalog for validation.
*   Basic input validation and error handling.
*   Swagger UI integration for easy API testing.
*   Unit tests for core service logic.

## Prerequisites

*   [.NET 7.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) or later.
*   Visual Studio 2022 (with ASP.NET and web development workload) or a compatible code editor (like VS Code).

## Getting Started

1.  **Clone or Download:** Get the source code.
2.  **Open in Visual Studio 2022:**
    *   Open the `Cart&checkout.sln` file (if provided) or open the `Cart&checkout` folder as a project.
    *   Ensure the `.NET 7.0` framework is selected.
3.  **Build the Solution:**
    *   Build the solution (Ctrl+Shift+B or Build > Build Solution) to restore NuGet packages.
4.  **Run the Project:**
    *   Press `F5` or click the "Start Debugging" button (often shows `Cart&checkout` or `https` as the launch profile).
    *   The application will start, and a browser window might open, likely navigating to the Swagger UI page (`/swagger/index.html`).

**Alternatively, using .NET CLI:**

```bash
# Navigate to the project directory
cd path/to/Cart&checkout

# Restore dependencies (if needed)
dotnet restore

# Build the project
dotnet build

# Run the project
dotnet run
```

The API will typically be available at `https://localhost:<port>` or `http://localhost:<port>` (check the console output for the exact URLs).

## Testing with Swagger UI

Once the application is running, navigate to `/swagger/index.html` in your browser (e.g., `https://localhost:7123/swagger/index.html`).

Swagger UI provides an interactive interface to:
*   View all available API endpoints.
*   See the expected request formats (including example JSON bodies).
*   Execute requests directly from the browser and view the responses.

## API Endpoints

All endpoints operate on a mock user ID (`testUser123`) defined in `InMemoryCartStorage.cs`.

**1. Add Item to Cart**
*   **URL:** `/api/cart/add`
*   **Method:** `POST`
*   **Request Body:**
    ```json
    {
      "productId": 1,
      "quantity": 2
    }
    ```
*   **Success Response (200 OK):** Returns the updated list of `CartItem` objects.
    ```json
    [
      {
        "productId": 1,
        "productName": "Laptop",
        "quantity": 2,
        "priceAtTimeOfAdd": 1200.00,
        "subtotal": 2400.00
      }
    ]
    ```
*   **Error Responses:**
    *   `400 Bad Request`: If quantity is invalid (<= 0) or request body is malformed.
    *   `404 Not Found`: If `productId` does not exist in the mock product repository.

**2. Remove Item from Cart**
*   **URL:** `/api/cart/remove/{productId}`
*   **Method:** `POST`
*   **URL Parameter:** `productId` (integer) - The ID of the product to remove.
*   **Query Parameter (Optional):** `quantity` (integer, default: 1) - The number of units to remove. If quantity is greater than or equal to the amount in the cart, the item is removed completely.
*   **Example:** `POST /api/cart/remove/1?quantity=1`
*   **Success Response (200 OK):** Returns the updated list of `CartItem` objects.
*   **Error Responses:**
    *   `400 Bad Request`: If `quantity` parameter is invalid (<= 0).

**3. View Cart**
*   **URL:** `/api/cart`
*   **Method:** `GET`
*   **Success Response (200 OK):** Returns the current list of `CartItem` objects in the cart.
    ```json
    [
      {
        "productId": 1,
        "productName": "Laptop",
        "quantity": 2,
        "priceAtTimeOfAdd": 1200.00,
        "subtotal": 2400.00
      },
      {
        "productId": 2,
        "productName": "Mouse",
        "quantity": 1,
        "priceAtTimeOfAdd": 25.50,
        "subtotal": 25.50
      }
    ]
    ```

**4. Mock Checkout**
*   **URL:** `/api/cart/checkout`
*   **Method:** `POST`
*   **Success Response (200 OK):** Returns a `CheckoutSummary` object and clears the cart.
    ```json
    {
      "itemsPurchased": [
        {
          "productId": 1,
          "productName": "Laptop",
          "quantity": 2,
          "priceAtTimeOfAdd": 1200.00,
          "subtotal": 2400.00
        },
        {
          "productId": 2,
          "productName": "Mouse",
          "quantity": 1,
          "priceAtTimeOfAdd": 25.50,
          "subtotal": 25.50
        }
      ],
      "totalCost": 2425.50,
      "confirmationMessage": "Payment Successful. Order processed."
    }
    ```
*   **Error Responses:**
    *   `400 Bad Request`: If the cart is empty.

## Running Unit Tests

Unit tests are located in the `InMemoryCartApi.Tests` project.

**Using Visual Studio Test Explorer:**
1.  Build the solution.
2.  Open the Test Explorer (Test > Test Explorer).
3.  Click "Run All Tests".

**Using .NET CLI:**

```bash
# Navigate to the solution directory (or the test project directory)
cd path/to/solution

# Run tests
dotnet test
```

## Project Structure

*   `/Controllers`: Contains API controllers (`CartController.cs`).
*   `/Data`: Contains in-memory storage simulation (`InMemoryCartStorage.cs`, `MockProductRepository.cs`).
*   `/Models`: Contains data models (`CartItem.cs`, `Product.cs`) and request DTOs (`AddToCartRequest.cs`).
*   `/Services`: Contains business logic interfaces and implementations (`ICartService.cs`, `CartService.cs`, `CheckoutSummary.cs`).
*   `/InMemoryCartApi.Tests`: Contains unit tests (`CartServiceTests.cs`).
*   `Program.cs`: Application entry point and service configuration.
*   `README.md`: This file.
