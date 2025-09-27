using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using GameNest.CatalogService.DAL.Data;
using GameNest.CatalogService.DAL.Repositories.Interfaces;
using GameNest.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameNest.CatalogService.DAL.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : BaseEntity
    {
        protected readonly CatalogDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        public IQueryable<TEntity> GetQueryable() => _dbSet.AsQueryable();
        public GenericRepository(CatalogDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<TEntity>();
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public virtual async Task<TEntity?> GetByIdAsync(Guid id, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            var query = asNoTracking ? _dbSet.AsNoTracking() : _dbSet;
            return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            var query = asNoTracking ? _dbSet.AsNoTracking() : _dbSet;
            return await query.ToListAsync(cancellationToken);
        }

        public virtual Task UpdateAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, asNoTracking: false, cancellationToken);
            if (entity != null)
                _dbSet.Remove(entity);
        }

        public virtual async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            var evaluator = new SpecificationEvaluator();
            var query = evaluator.GetQuery(_dbSet.AsQueryable(), spec).AsNoTracking();
            return await query.ToListAsync(cancellationToken);
        }

        protected IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));

            var evaluator = new SpecificationEvaluator();
            return evaluator
                .GetQuery(_dbSet.AsQueryable(), spec)
                .AsSplitQuery();
        }
    }
}