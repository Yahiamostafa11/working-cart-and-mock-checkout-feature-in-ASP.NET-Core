using Microsoft.VisualStudio.TestTools.UnitTesting;
using InMemoryCartApi.Services;
using InMemoryCartApi.Data;
using InMemoryCartApi.Models;
using System.Linq;
using System.Collections.Generic;
using System;

namespace InMemoryCartApi.Tests
{
    [TestClass]
    public class CartServiceTests
    {
        private ICartService _cartService = null!;
        private string _testUserId = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            // Reset storage and service for each test to ensure isolation
            _testUserId = "testUser_" + Guid.NewGuid().ToString(); // Unique user ID per test
            InMemoryCartStorage.ClearCart(_testUserId); // Ensure clean slate
            // Ensure mock user cart exists if needed, though we use unique IDs here
            // InMemoryCartStorage.EnsureMockUserCartExists(); 
            _cartService = new CartService(); // Create a new service instance
        }

        // --- AddToCart Tests ---

        [TestMethod]
        public void AddToCart_AddNewItem_Success()
        {
            // Arrange
            int productId = 1; // Laptop
            int quantity = 1;

            // Act
            var cart = _cartService.AddToCart(_testUserId, productId, quantity);

            // Assert
            Assert.IsNotNull(cart);
            Assert.AreEqual(1, cart.Count);
            var addedItem = cart.First();
            Assert.AreEqual(productId, addedItem.ProductId);
            Assert.AreEqual(quantity, addedItem.Quantity);
            Assert.AreEqual("Laptop", addedItem.ProductName);
            Assert.AreEqual(1200.00m, addedItem.PriceAtTimeOfAdd);
        }

        [TestMethod]
        public void AddToCart_AddExistingItem_IncrementsQuantity()
        {
            // Arrange
            int productId = 2; // Mouse
            _cartService.AddToCart(_testUserId, productId, 1); // Add 1 initially

            // Act
            var cart = _cartService.AddToCart(_testUserId, productId, 2); // Add 2 more

            // Assert
            Assert.AreEqual(1, cart.Count); // Still only one item type
            var item = cart.First();
            Assert.AreEqual(productId, item.ProductId);
            Assert.AreEqual(3, item.Quantity); // Initial 1 + Added 2
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddToCart_ZeroQuantity_ThrowsArgumentException()
        {
            // Act
            _cartService.AddToCart(_testUserId, 1, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddToCart_NegativeQuantity_ThrowsArgumentException()
        {
            // Act
            _cartService.AddToCart(_testUserId, 1, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void AddToCart_InvalidProductId_ThrowsKeyNotFoundException()
        {
            // Act
            _cartService.AddToCart(_testUserId, 999, 1); // 999 is not a valid product ID
        }

        // --- RemoveFromCart Tests ---

        [TestMethod]
        public void RemoveFromCart_DecrementQuantity_Success()
        {
            // Arrange
            int productId = 3; // Keyboard
            _cartService.AddToCart(_testUserId, productId, 3); // Add 3 keyboards

            // Act
            var cart = _cartService.RemoveFromCart(_testUserId, productId, 1); // Remove 1

            // Assert
            Assert.AreEqual(1, cart.Count);
            var item = cart.First();
            Assert.AreEqual(productId, item.ProductId);
            Assert.AreEqual(2, item.Quantity); // 3 - 1 = 2
        }

        [TestMethod]
        public void RemoveFromCart_RemoveExactQuantity_RemovesItem()
        {
            // Arrange
            int productId = 4; // Monitor
            _cartService.AddToCart(_testUserId, productId, 2);

            // Act
            var cart = _cartService.RemoveFromCart(_testUserId, productId, 2); // Remove all 2

            // Assert
            Assert.AreEqual(0, cart.Count); // Cart should be empty
        }

        [TestMethod]
        public void RemoveFromCart_RemoveMoreThanQuantity_RemovesItem()
        {
            // Arrange
            int productId = 5; // Webcam
            _cartService.AddToCart(_testUserId, productId, 1);

            // Act
            var cart = _cartService.RemoveFromCart(_testUserId, productId, 5); // Try removing 5

            // Assert
            Assert.AreEqual(0, cart.Count); // Item should be removed
        }

        [TestMethod]
        public void RemoveFromCart_ItemNotInCart_DoesNothing()
        {
            // Arrange
            _cartService.AddToCart(_testUserId, 1, 1); // Add a different item
            var initialCart = _cartService.GetCart(_testUserId);
            int initialCount = initialCart.Count;

            // Act
            var cart = _cartService.RemoveFromCart(_testUserId, 999, 1); // Try removing non-existent item

            // Assert
            Assert.AreEqual(initialCount, cart.Count); // Count should not change
            // Optionally check contents are identical if needed
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemoveFromCart_ZeroQuantity_ThrowsArgumentException()
        {
            // Arrange
            _cartService.AddToCart(_testUserId, 1, 1);

            // Act
            _cartService.RemoveFromCart(_testUserId, 1, 0);
        }

        // --- GetCart Tests ---

        [TestMethod]
        public void GetCart_EmptyCart_ReturnsEmptyList()
        {
            // Act
            var cart = _cartService.GetCart(_testUserId);

            // Assert
            Assert.IsNotNull(cart);
            Assert.AreEqual(0, cart.Count);
        }

        [TestMethod]
        public void GetCart_WithItems_ReturnsCorrectItems()
        {
            // Arrange
            _cartService.AddToCart(_testUserId, 1, 1);
            _cartService.AddToCart(_testUserId, 2, 3);

            // Act
            var cart = _cartService.GetCart(_testUserId);

            // Assert
            Assert.AreEqual(2, cart.Count);
            Assert.IsTrue(cart.Any(i => i.ProductId == 1 && i.Quantity == 1));
            Assert.IsTrue(cart.Any(i => i.ProductId == 2 && i.Quantity == 3));
        }

        // --- Checkout Tests ---

        [TestMethod]
        public void Checkout_ValidCart_ReturnsSummaryAndClearsCart()
        {
            // Arrange
            _cartService.AddToCart(_testUserId, 1, 1); // Laptop: 1 * 1200.00 = 1200.00
            _cartService.AddToCart(_testUserId, 2, 2); // Mouse: 2 * 25.50 = 51.00
            decimal expectedTotal = 1200.00m + 51.00m;

            // Act
            var summary = _cartService.Checkout(_testUserId);
            var cartAfterCheckout = _cartService.GetCart(_testUserId);

            // Assert
            Assert.IsNotNull(summary);
            Assert.AreEqual(2, summary.ItemsPurchased.Count);
            Assert.AreEqual(expectedTotal, summary.TotalCost);
            Assert.AreEqual("Payment Successful. Order processed.", summary.ConfirmationMessage);
            Assert.AreEqual(0, cartAfterCheckout.Count); // Cart should be empty
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Checkout_EmptyCart_ThrowsInvalidOperationException()
        {
            // Act
            _cartService.Checkout(_testUserId);
        }
    }
}
