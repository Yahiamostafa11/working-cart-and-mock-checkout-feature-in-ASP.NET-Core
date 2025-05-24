using InMemoryCartApi.Data;
using InMemoryCartApi.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InMemoryCartApi.Services
{
    public class CartService : ICartService
    {
        // Note: In a real app, these static dependencies would ideally be injected.
        // For this exercise, we directly use the static classes as per the simplified setup.

        public List<CartItem> AddToCart(string userId, int productId, int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be positive.", nameof(quantity));
            }

            var product = MockProductRepository.GetProductById(productId);
            if (product == null)
            {
                // Using a specific exception type can be helpful for error handling middleware
                throw new KeyNotFoundException($"Product with ID {productId} not found.");
            }

            var cart = InMemoryCartStorage.GetCart(userId);
            var existingItem = cart.FirstOrDefault(item => item.ProductId == productId);

            if (existingItem != null)
            {
                // Item already in cart, update quantity
                existingItem.Quantity += quantity;
                // Optionally update price if it can change, but requirement implies price at time of add
                // existingItem.PriceAtTimeOfAdd = product.Price; 
            }
            else
            {
                // Add new item to cart
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name, // Store name for convenience
                    Quantity = quantity,
                    PriceAtTimeOfAdd = product.Price // Store price at the time it was added
                });
            }

            InMemoryCartStorage.UpdateCart(userId, cart);
            return cart;
        }

        public List<CartItem> RemoveFromCart(string userId, int productId, int quantityToRemove = 1)
        {
            if (quantityToRemove <= 0)
            {
                throw new ArgumentException("Quantity to remove must be positive.", nameof(quantityToRemove));
            }

            var cart = InMemoryCartStorage.GetCart(userId);
            var itemToRemove = cart.FirstOrDefault(item => item.ProductId == productId);

            if (itemToRemove != null)
            {
                if (itemToRemove.Quantity > quantityToRemove)
                {
                    // Decrement quantity
                    itemToRemove.Quantity -= quantityToRemove;
                }
                else
                {
                    // Remove the item entirely (quantity <= quantityToRemove)
                    cart.Remove(itemToRemove);
                }

                InMemoryCartStorage.UpdateCart(userId, cart);
            }
            // If item not found, we could throw an exception or just return the current cart
            // else { throw new KeyNotFoundException($"Product with ID {productId} not found in cart."); }

            return cart;
        }

        public List<CartItem> GetCart(string userId)
        {
            // Simply return the current state of the cart
            return InMemoryCartStorage.GetCart(userId);
        }

        public CheckoutSummary Checkout(string userId)
        {
            var cart = InMemoryCartStorage.GetCart(userId);

            if (!cart.Any())
            {
                throw new InvalidOperationException("Cannot checkout with an empty cart.");
            }

            // Calculate total cost
            decimal totalCost = cart.Sum(item => item.Subtotal);

            // Create summary (make a copy of items in case cart is modified elsewhere)
            var summary = new CheckoutSummary
            {
                ItemsPurchased = new List<CartItem>(cart.Select(i => new CartItem
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    PriceAtTimeOfAdd = i.PriceAtTimeOfAdd
                })), // Deep copy might be needed depending on CartItem complexity
                TotalCost = totalCost,
                ConfirmationMessage = "Payment Successful. Order processed."
            };

            // Clear the user's cart after successful checkout
            InMemoryCartStorage.ClearCart(userId);

            return summary;
        }
    }
}
