
using System.Linq.Expressions;

namespace EMS.Data.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAll();

        IQueryable<TEntity> GetAllAsync();
        IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> GetWithInclude(Expression<Func<TEntity, bool>> predicate, string include);
        Task<IEnumerable<TEntity>> GetWithIncludeAsync(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity> GetById(int id);
        bool Add(TEntity entity);
        Task<bool> AddRange(IEnumerable<TEntity> entities);
        bool Update(TEntity entity);
        bool Delete(int id);

    }

}
