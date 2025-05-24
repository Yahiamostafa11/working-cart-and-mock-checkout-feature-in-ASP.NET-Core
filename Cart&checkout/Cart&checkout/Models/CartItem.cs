namespace InMemoryCartApi.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty; // Added for easier display
        public int Quantity { get; set; }
        public decimal PriceAtTimeOfAdd { get; set; } // Price per unit
        public decimal Subtotal => Quantity * PriceAtTimeOfAdd; // Calculated property
    }
}
