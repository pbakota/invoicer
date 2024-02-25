using Invoicer.Data.Utils;
using Invoicer.Models;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Logging;
using Invoicer.Data.Extensions;

namespace Invoicer.Data.Dao;

public interface ICrudRepository<TEntity>
    where TEntity : class, IEntity
{
    Task<TEntity?> FindById(int entityId);
    Task<TEntity> UpdateById(int entityId, TEntity entity);
    Task DeleteById(int entityId);
    Task<TEntity> Create(TEntity entity);
    Task<PagedResult<TEntity>> FindAllPaged(Pageable pageable);
}

public abstract class CrudRepository<TEntity>(IDbContextFactory<InvoicerContext> contextFactory, ILogger<TEntity> logger)
    : ICrudRepository<TEntity>
    where TEntity : class, IEntity
{
    protected readonly IDbContextFactory<InvoicerContext> _contextFactory = contextFactory;

    protected readonly ILogger<TEntity> _logger = logger;

    public virtual async Task<TEntity> Create(TEntity newEntity)
    {
        using var db = _contextFactory.CreateDbContext();

        db.Entry(newEntity).State = EntityState.Added;
        await db.SaveChangesAsync();

        return newEntity;
    }

    public async Task DeleteById(int entityId)
    {
        using var db = _contextFactory.CreateDbContext();
        var entity = await db.Set<TEntity>().SingleOrDefaultAsync(x => x.Id == entityId);
        if (entity is null) return;

        db.Set<TEntity>().Remove(entity);
        await db.SaveChangesAsync();
    }

    public virtual async Task<PagedResult<TEntity>> FindAllPaged(Pageable pageable)
    {
        using var db = _contextFactory.CreateDbContext();
        var query = db.Set<TEntity>().AsNoTracking().ApplySearchAndOrder(pageable);
        var total = await query.CountAsync();

        query = query.ApplyPagination(pageable);
        return new PagedResult<TEntity>
        {
            Content = await query.ToListAsync(),
            Total = total,
        };
    }

    public virtual async Task<TEntity?> FindById(int entityId)
    {
        using var db = _contextFactory.CreateDbContext();
        var entity = await db.Set<TEntity>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == entityId);

        return entity;
    }

    public virtual async Task<TEntity> UpdateById(int entityId, TEntity entity)
    {
        using var db = _contextFactory.CreateDbContext();
        entity.Id = entityId;

        db.Set<TEntity>().Update(entity);
        await db.SaveChangesAsync();

        return entity;
    }
}