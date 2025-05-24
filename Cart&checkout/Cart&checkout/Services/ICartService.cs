using InMemoryCartApi.Models;

namespace InMemoryCartApi.Services
{
    // Defines the contract for cart operations
    public interface ICartService
    {
        // Adds an item to the specified user's cart.
        // Returns the updated cart or null/throws exception on failure (e.g., invalid product).
        List<CartItem> AddToCart(string userId, int productId, int quantity);

        // Removes an item (or decrements quantity) from the specified user's cart.
        // Returns the updated cart.
        List<CartItem> RemoveFromCart(string userId, int productId, int quantityToRemove = 1); // Default to removing 1

        // Retrieves the current cart for the specified user.
        List<CartItem> GetCart(string userId);

        // Processes the checkout for the specified user's cart.
        // Returns an order summary or confirmation details.
        // Throws exception or returns specific result for empty cart.
        CheckoutSummary Checkout(string userId);
    }

    // Simple DTO for checkout result
    public class CheckoutSummary
    {
        public List<CartItem> ItemsPurchased { get; set; } = new List<CartItem>();
        public decimal TotalCost { get; set; }
        public string ConfirmationMessage { get; set; } = string.Empty;
    }
}
