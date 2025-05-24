using InMemoryCartApi.Models;

namespace InMemoryCartApi.Data
{
    // Using a static class for simplicity. Could be a Singleton service.
    public static class MockProductRepository
    {
        private static readonly List<Product> _products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Price = 1200.00m },
            new Product { Id = 2, Name = "Mouse", Price = 25.50m },
            new Product { Id = 3, Name = "Keyboard", Price = 75.00m },
            new Product { Id = 4, Name = "Monitor", Price = 300.75m },
            new Product { Id = 5, Name = "Webcam", Price = 50.00m }
        };

        public static Product? GetProductById(int productId)
        {
            return _products.FirstOrDefault(p => p.Id == productId);
        }

        public static bool IsValidProduct(int productId)
        {
            return _products.Any(p => p.Id == productId);
        }

        // Optional: Get all products if needed elsewhere
        public static List<Product> GetAllProducts()
        {
            return _products;
        }
    }
}

