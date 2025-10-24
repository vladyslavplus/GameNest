using System.ComponentModel.DataAnnotations;

namespace GameNest.OrderService.BLL.DTOs.Order
{
    public class OrderCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Street { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string ZipCode { get; set; } = null!;
    }
}
