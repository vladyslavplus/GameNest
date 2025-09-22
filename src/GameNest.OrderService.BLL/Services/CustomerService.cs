using AutoMapper;
using GameNest.OrderService.BLL.DTOs.Customer;
using GameNest.OrderService.BLL.Services.Interfaces;
using GameNest.OrderService.DAL.UOW;
using GameNest.OrderService.Domain.Entities;
using GameNest.OrderService.Domain.Exceptions;

namespace GameNest.OrderService.BLL.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CustomerDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var customer = await _unitOfWork.Customers!.GetByIdAsync(id, ct);

            if (customer == null)
                throw new NotFoundException($"Customer with id {id} not found.");

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync(CancellationToken ct = default)
        {
            var customers = await _unitOfWork.Customers!.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        public async Task<CustomerDto> CreateAsync(CustomerCreateDto dto, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new ValidationException("Username cannot be empty.");
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ValidationException("Email cannot be empty.");

            var existingByEmail = await _unitOfWork.Customers!.GetByEmailAsync(dto.Email, ct);
            if (existingByEmail != null)
                throw new ValidationException($"Customer with email {dto.Email} already exists.");

            var existingByUsername = await _unitOfWork.Customers.GetByUsernameAsync(dto.Username, ct);
            if (existingByUsername != null)
                throw new ValidationException($"Customer with username {dto.Username} already exists.");

            var entity = _mapper.Map<Customer>(dto);
            var id = await _unitOfWork.Customers.CreateAsync(entity, ct);
            await _unitOfWork.CommitAsync(ct);

            var created = await _unitOfWork.Customers.GetByIdAsync(id, ct);
            return _mapper.Map<CustomerDto>(created);
        }

        public async Task<CustomerDto> UpdateAsync(Guid id, CustomerUpdateDto dto, CancellationToken ct = default)
        {
            var customer = await _unitOfWork.Customers!.GetByIdAsync(id, ct);
            if (customer == null)
                throw new NotFoundException($"Customer with id {id} not found.");

            bool hasChanges = false;

            if (!string.IsNullOrWhiteSpace(dto.Username) && dto.Username != customer.Username)
            {
                var existing = await _unitOfWork.Customers.GetByUsernameAsync(dto.Username, ct);
                if (existing != null && existing.Id != id)
                    throw new ValidationException($"Username {dto.Username} is already taken.");

                customer.Username = dto.Username;
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != customer.Email)
            {
                var existing = await _unitOfWork.Customers.GetByEmailAsync(dto.Email, ct);
                if (existing != null && existing.Id != id)
                    throw new ValidationException($"Email {dto.Email} is already taken.");

                customer.Email = dto.Email;
                hasChanges = true;
            }

            if (!hasChanges)
                throw new ValidationException("No changes detected for the update.");

            customer.Updated_At = DateTime.UtcNow;

            await _unitOfWork.Customers.UpdateAsync(customer, ct);
            await _unitOfWork.CommitAsync(ct);

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task DeleteAsync(Guid id, bool softDelete = true, CancellationToken ct = default)
        {
            var customer = await _unitOfWork.Customers!.GetByIdAsync(id, ct);
            if (customer == null)
                throw new NotFoundException($"Customer with id {id} not found.");

            await _unitOfWork.Customers.DeleteAsync(customer.Id, softDelete, ct);
            await _unitOfWork.CommitAsync(ct);
        }
    }
}
