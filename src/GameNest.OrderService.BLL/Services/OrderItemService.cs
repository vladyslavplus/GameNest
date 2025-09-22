using AutoMapper;
using GameNest.OrderService.BLL.DTOs.OrderItem;
using GameNest.OrderService.BLL.Services.Interfaces;
using GameNest.OrderService.DAL.UOW;
using GameNest.OrderService.Domain.Exceptions;

namespace GameNest.OrderService.BLL.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderItemService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OrderItemDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var item = await _unitOfWork.OrderItems!.GetByIdAsync(id, ct);

            if (item == null)
                throw new NotFoundException($"OrderItem with id {id} not found.");

            return _mapper.Map<OrderItemDto>(item);
        }

        public async Task<IEnumerable<OrderItemDto>> GetAllAsync(CancellationToken ct = default)
        {
            var items = await _unitOfWork.OrderItems!.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<OrderItemDto>>(items);
        }

        public async Task<IEnumerable<OrderItemDto>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default)
        {
            var items = await _unitOfWork.OrderItems!.GetByOrderIdAsync(orderId, ct);
            return _mapper.Map<IEnumerable<OrderItemDto>>(items);
        }
    }
}
