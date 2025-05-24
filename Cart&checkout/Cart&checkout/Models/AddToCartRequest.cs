using System.ComponentModel.DataAnnotations;

namespace InMemoryCartApi.Models
{
    // DTO for the Add to Cart request body
    public class AddToCartRequest
    {
        [Required] // Ensure ProductId is provided
        public int ProductId { get; set; }

        [Required] // Ensure Quantity is provided
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")] // Ensure Quantity is positive
        public int Quantity { get; set; }
    }
}
