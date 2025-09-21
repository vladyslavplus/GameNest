using AutoMapper;
using GameNest.OrderService.BLL.DTOs.Product;
using GameNest.OrderService.BLL.Services.Interfaces;
using GameNest.OrderService.DAL.UOW;
using GameNest.OrderService.Domain.Entities;
using GameNest.OrderService.Domain.Exceptions;

namespace GameNest.OrderService.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var product = await _unitOfWork.Products!.GetByIdAsync(id, ct);

            if (product == null)
                throw new NotFoundException($"Product with id {id} not found.");

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken ct = default)
        {
            var products = await _unitOfWork.Products!.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto> CreateAsync(ProductCreateDto dto, CancellationToken ct = default)
        {
            if (dto.Price <= 0)
                throw new ValidationException("Price must be greater than 0.");

            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ValidationException("Product title cannot be empty.");

            var entity = _mapper.Map<Product>(dto);

            var id = await _unitOfWork.Products!.CreateAsync(entity, ct);
            await _unitOfWork.CommitAsync(ct);

            var created = await _unitOfWork.Products.GetByIdAsync(id, ct);
            return _mapper.Map<ProductDto>(created);
        }

        public async Task<ProductDto> UpdateAsync(Guid id, ProductUpdateDto dto, CancellationToken ct = default)
        {
            var product = await _unitOfWork.Products!.GetByIdAsync(id, ct);
            if (product == null)
                throw new NotFoundException($"Product with id {id} not found.");

            bool hasChanges = false;

            if (dto.Title != null && dto.Title != product.Title)
            {
                product.Title = dto.Title;
                hasChanges = true;
            }

            if (dto.Description != null && dto.Description != product.Description)
            {
                product.Description = dto.Description;
                hasChanges = true;
            }

            if (dto.Price.HasValue && dto.Price.Value != product.Price)
            {
                if (dto.Price.Value <= 0)
                    throw new ValidationException("Price must be greater than 0.");

                product.Price = dto.Price.Value;
                hasChanges = true;
            }

            if (!hasChanges)
                throw new ValidationException("No changes detected for the update.");

            product.Updated_At = DateTime.UtcNow;

            await _unitOfWork.Products.UpdateAsync(product, ct);
            await _unitOfWork.CommitAsync(ct);

            return _mapper.Map<ProductDto>(product);
        }

        public async Task DeleteAsync(Guid id, bool softDelete = true, CancellationToken ct = default)
        {
            var product = await _unitOfWork.Products!.GetByIdAsync(id, ct);
            if (product == null)
                throw new NotFoundException($"Product with id {id} not found.");

            await _unitOfWork.Products.DeleteAsync(product.Id, softDelete, ct);
            await _unitOfWork.CommitAsync(ct);
        }
    }
}