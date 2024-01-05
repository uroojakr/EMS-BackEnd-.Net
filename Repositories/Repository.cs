using EMS.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;


namespace EMS.Data
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly EMSDbContext _context;
        private readonly DbSet<TEntity> _dbSet;
        private readonly ILogger<Repository<TEntity>> _logger;

        public Repository(EMSDbContext context, ILogger<Repository<TEntity>> logger)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
            _logger = logger;
        }

        public bool Add(TEntity entity)
        {
            try
            {
                _dbSet.Add(entity);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorMessage}", ex.Message);
                return false;
            }
        }
        public async Task<bool> AddRange(IEnumerable<TEntity> entities)
        {
            try
            {
                await _dbSet.AddRangeAsync(entities);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorMessage}", ex.Message);
                return false;
            }
        }
        public bool Delete(int id)
        {
            try
            {
                var entity = _dbSet.Find(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorMessage}", ex.Message);
                return false;
            }
        }
        public async Task<TEntity> GetById(int id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                return entity!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorMessage}", ex.Message);
                return null!;
            }
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }
        public IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }
        public async Task<IEnumerable<TEntity>> GetWithInclude(Expression<Func<TEntity, bool>> predicate, string include)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrEmpty(include))
            {
                query = query.Include(include);
            }

            return await query.Where(predicate).ToListAsync();
        }
        public bool Update(TEntity entity)
        {
            try
            {
                _dbSet.Update(entity);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorMessage}", ex.Message);
                return false;
            }
        }
        public async Task<IEnumerable<TEntity>> GetWithIncludeAsync(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _dbSet;

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.Where(filter).ToListAsync();
        }

        public IQueryable<TEntity> GetAllAsync()
        {
            return _dbSet;
        }
    }
}

