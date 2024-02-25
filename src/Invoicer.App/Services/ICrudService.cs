using Invoicer.Data.Utils;
using Invoicer.Models;

namespace Invoicer.App.Services;

public interface ICrudService<TEntity> where TEntity : class, IEntity
{
    Pageable Pageable { get; }
    Task<TEntity?> GetSingle(int articleId);
    Task<PagedResult<TEntity>> GetPagedData();
}
