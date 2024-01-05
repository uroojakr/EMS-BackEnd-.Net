
namespace EMS.Business.Interfaces
{
    public interface IGenericCrudService<TEntity, TModel>

    {
        Task<IEnumerable<TModel>> GetAllAsync();
        Task<TModel> GetByIdAsync(int id);
        Task<bool> CreateAsync(TModel model);
        Task<bool> UpdateAsync(int id, TModel model);
        Task<bool> DeleteAsync(int id);
    }
}