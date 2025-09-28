using AutoMapper;
using GameNest.OrderService.BLL.DTOs.PaymentRecord;
using GameNest.OrderService.BLL.Services.Interfaces;
using GameNest.OrderService.DAL.UOW;
using GameNest.OrderService.Domain.Entities;
using GameNest.OrderService.Domain.Exceptions;

namespace GameNest.OrderService.BLL.Services
{
    public class PaymentRecordService : IPaymentRecordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentRecordService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaymentRecordDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var paymentRecordEntity = await _unitOfWork.PaymentRecords!.GetByIdAsync(id, ct);
            if (paymentRecordEntity == null)
                throw new NotFoundException($"Payment record with id {id} not found.");

            return _mapper.Map<PaymentRecordDto>(paymentRecordEntity);
        }

        public async Task<IEnumerable<PaymentRecordDto>> GetAllAsync(CancellationToken ct = default)
        {
            var paymentRecordEntities = await _unitOfWork.PaymentRecords!.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<PaymentRecordDto>>(paymentRecordEntities);
        }

        public async Task<IEnumerable<PaymentRecordDto>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default)
        {
            var paymentRecordEntities = await _unitOfWork.PaymentRecords!.GetByOrderIdAsync(orderId, ct);
            return _mapper.Map<IEnumerable<PaymentRecordDto>>(paymentRecordEntities);
        }

        public async Task<PaymentRecordDto> CreateAsync(PaymentRecordCreateDto dto, CancellationToken ct = default)
        {
            var orderEntity = await _unitOfWork.Orders!.GetByIdAsync(dto.Order_Id, ct);
            if (orderEntity == null)
                throw new ValidationException($"Order with id {dto.Order_Id} does not exist.");

            var existingPayments = await _unitOfWork.PaymentRecords!.GetByOrderIdAsync(dto.Order_Id, ct);
            if (existingPayments.Any())
                throw new ValidationException($"A payment for order {dto.Order_Id} already exists.");

            var paymentRecordEntity = new PaymentRecord
            {
                Order_Id = dto.Order_Id,
                Method = dto.Method,
                Amount = dto.Amount,
                Status = "Pending"
            };

            Guid id;

            try
            {
                id = await _unitOfWork.PaymentRecords.CreateAsync(paymentRecordEntity, ct);
                await _unitOfWork.CommitAsync(ct);

                paymentRecordEntity.Id = id;
                paymentRecordEntity.Status = "Success";
                paymentRecordEntity.Updated_At = DateTime.UtcNow;

                await _unitOfWork.PaymentRecords.UpdateAsync(paymentRecordEntity, ct);

                orderEntity.Status = "Paid";
                orderEntity.Updated_At = DateTime.UtcNow;
                await _unitOfWork.Orders.UpdateAsync(orderEntity, ct);

                await _unitOfWork.CommitAsync(ct);
            }
            catch
            {
                paymentRecordEntity.Status = "Failed";
                paymentRecordEntity.Updated_At = DateTime.UtcNow;

                await _unitOfWork.PaymentRecords.CreateAsync(paymentRecordEntity, ct);
                await _unitOfWork.CommitAsync(ct);

                throw; 
            }

            var createdEntity = await _unitOfWork.PaymentRecords.GetByIdAsync(id, ct);
            return _mapper.Map<PaymentRecordDto>(createdEntity);
        }

        public async Task<PaymentRecordDto> UpdateAsync(Guid id, PaymentRecordUpdateDto dto, CancellationToken ct = default)
        {
            var paymentRecordEntity = await _unitOfWork.PaymentRecords!.GetByIdAsync(id, ct);
            if (paymentRecordEntity == null)
                throw new NotFoundException($"Payment record with id {id} not found.");

            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new ValidationException("Status cannot be empty.");

            if (dto.Status == paymentRecordEntity.Status)
                throw new ValidationException("No changes detected for the update.");

            paymentRecordEntity.Status = dto.Status;
            paymentRecordEntity.Updated_At = DateTime.UtcNow;

            await _unitOfWork.PaymentRecords.UpdateAsync(paymentRecordEntity, ct);
            await _unitOfWork.CommitAsync(ct);

            return _mapper.Map<PaymentRecordDto>(paymentRecordEntity);
        }

        public async Task DeleteAsync(Guid id, bool softDelete = true, CancellationToken ct = default)
        {
            var paymentRecordEntity = await _unitOfWork.PaymentRecords!.GetByIdAsync(id, ct);
            if (paymentRecordEntity == null)
                throw new NotFoundException($"Payment record with id {id} not found.");

            await _unitOfWork.PaymentRecords.DeleteAsync(id, softDelete, ct);
            await _unitOfWork.CommitAsync(ct);
        }
    }
}