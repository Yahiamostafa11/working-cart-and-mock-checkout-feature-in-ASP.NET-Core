using InMemoryCartApi.Models;
using System.Collections.Concurrent; // Using ConcurrentDictionary for thread safety

namespace InMemoryCartApi.Data
{
    // Using a static class for simplicity in this example.
    // For a real application, consider a Singleton service registered via DI.
    public static class InMemoryCartStorage
    {
        // Using ConcurrentDictionary for basic thread safety if multiple requests modify carts.
        // Key: Mock User ID (string), Value: List of CartItem
        private static readonly ConcurrentDictionary<string, List<CartItem>> _userCarts = new ConcurrentDictionary<string, List<CartItem>>();

        // Mock User ID for testing purposes as per requirements
        public const string MockUserId = "testUser123";

        public static List<CartItem> GetCart(string userId)
        {
            // Ensure the cart exists for the user, return empty list if not
            return _userCarts.GetOrAdd(userId, _ => new List<CartItem>());
        }

        public static void UpdateCart(string userId, List<CartItem> cart)
        {
            // Overwrites the existing cart for the user
            _userCarts[userId] = cart;
        }

        public static void ClearCart(string userId)
        {
            // Removes the user's cart entry
            _userCarts.TryRemove(userId, out _);
            // Or alternatively, update with an empty list if you prefer to keep the key:
            // if (_userCarts.ContainsKey(userId)) { _userCarts[userId] = new List<CartItem>(); }
        }

        // Helper to ensure the mock user's cart is initialized if needed elsewhere
        public static void EnsureMockUserCartExists()
        {
            _userCarts.TryAdd(MockUserId, new List<CartItem>());
        }
    }
}

