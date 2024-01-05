using AutoMapper;
using EMS.Business.Interfaces;
using EMS.Data.Interfaces;

public class GenericCrudService<TEntity, TModel> : IGenericCrudService<TEntity, TModel>
    where TEntity : class
    where TModel : class
{
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly IMapper _mapper;
    protected readonly IRepository<TEntity> _repository;

    public GenericCrudService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _repository = _unitOfWork.GetRepository<TEntity>();
    }

    public async Task<IEnumerable<TModel>> GetAllAsync()
    {
        var entities = await _repository.GetAll();
        return _mapper.Map<IEnumerable<TModel>>(entities);
    }

    public async Task<TModel> GetByIdAsync(int id)
    {
        var entity = await _repository.GetById(id);
        return _mapper.Map<TModel>(entity);
    }

    public async Task<bool> CreateAsync(TModel model)
    {
        var entity = _mapper.Map<TEntity>(model);
        _repository.Add(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(int id, TModel model)
    {
        var entity = await _repository.GetById(id);

        if (entity == null)
        {
            return false;
        }
        var a = _mapper.Map(model, entity);

        _repository.Update(a);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _repository.Delete(id);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    protected virtual int GetEntityId(TEntity entity)
    {
        var idProperty = entity.GetType().GetProperty("Id");
        return (int)(idProperty?.GetValue(entity) ?? throw new InvalidOperationException("The GetEntityId method is not correct"));
    }
}

