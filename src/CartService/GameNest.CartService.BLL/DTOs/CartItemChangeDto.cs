using System.ComponentModel.DataAnnotations;

namespace GameNest.CartService.BLL.DTOs
{
    public class CartItemChangeDto
    {
        [Required]
        public Guid ProductId { get; set; }

        [Range(-100, 100, ErrorMessage = "Quantity change must be between -100 and 100")]
        public int Quantity { get; set; }
    }
}
