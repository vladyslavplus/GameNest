namespace GameNest.OrderService.BLL.DTOs.Product
{
    public class ProductCreateDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
}
