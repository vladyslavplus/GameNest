using System.ComponentModel.DataAnnotations;

namespace GameNest.CartService.BLL.DTOs
{
    public class CartItemAddDto
    {
        [Required]
        public Guid ProductId { get; set; }

        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, 10000, ErrorMessage = "Price must be valid")]
        public decimal Price { get; set; }
    }
}
