using Invoicer.Data.Extensions;

using Invoicer.Data.Utils;
using Invoicer.Models;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Logging;

namespace Invoicer.Data.Dao;

public interface IInvoiceRepository : ICrudRepository<Invoice>
{
    Task<int> GetNextInvoiceId(InvoiceType invoiceType);
    Task StornoInvoice(int invoiceId);
    Task<PagedResult<Invoice>> FindAllPrepayedPaged(Pageable pageable);
}

public class InvoiceRepository(IDbContextFactory<InvoicerContext> contextFactory, ILogger<Invoice> logger, ISettingsRepository settingsRepository)
    : CrudRepository<Invoice>(contextFactory, logger), IInvoiceRepository
{
    private readonly ISettingsRepository _settingsRepository = settingsRepository;

    public override async Task<Invoice> Create(Invoice newEntity)
    {
        using var db = _contextFactory.CreateDbContext();
        db.Set<Invoice>().Add(newEntity);
        await db.SaveChangesAsync();
        return newEntity;
    }

    public override async Task<Invoice> UpdateById(int entityId, Invoice entity)
    {
        using var db = _contextFactory.CreateDbContext();
        entity.Id = entityId;
        db.Set<Invoice>().Update(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public override async Task<PagedResult<Invoice>> FindAllPaged(Pageable pageable)
    {
        using var db = _contextFactory.CreateDbContext();
        var settings = await _settingsRepository.FindFirst();
        var query = db.Set<Invoice>().AsNoTracking().Include(x => x.Partner).ApplySearchAndOrder(pageable);

        if (settings.HideStornoInvoice)
        {
            query = query.Where(x => !x.Storno);
        }
        var total = await query.CountAsync();

        query = query.ApplyPagination(pageable);
        return new PagedResult<Invoice>
        {
            Content = await query.ToListAsync(),
            Total = total,
        };
    }
    public override async Task<Invoice?> FindById(int entityId)
    {
        using var db = _contextFactory.CreateDbContext();
        return await db.Set<Invoice>().AsNoTracking().Include(x => x.Partner).Include(x => x.Items).ThenInclude(x => x.Article).FirstOrDefaultAsync(x => x.Id == entityId);
    }

    public async Task<int> GetNextInvoiceId(InvoiceType invoiceType)
    {
        using var db = _contextFactory.CreateDbContext();
        return await db.Set<Invoice>().AsNoTracking().Where(x => x.Year == DateTime.UtcNow.Year && x.InvoiceType == invoiceType)
            .MaxAsync(x => (int?)x.NumberId) ?? 0;
    }

    public async Task StornoInvoice(int invoiceId)
    {
        using var db = _contextFactory.CreateDbContext();
        var invoice = await db.Set<Invoice>().FirstOrDefaultAsync(x => x.Id == invoiceId);
        if (invoice is null) return;
        invoice.Storno = true;
        await db.SaveChangesAsync();
    }

    public async Task<PagedResult<Invoice>> FindAllPrepayedPaged(Pageable pageable)
    {
        using var db = _contextFactory.CreateDbContext();
        var settings = await _settingsRepository.FindFirst();
        var query = db.Set<Invoice>().AsNoTracking().Include(x => x.Partner)
            .Where(f => f.InvoiceType == InvoiceType.PREPAYMENT).ApplySearchAndOrder(pageable);

        query = query.Where(f => !db.Set<Invoice>().Any(p => p.PrepaymentId == f.Id));

        if (settings.HideStornoInvoice)
        {
            query = query.Where(x => !x.Storno);
        }
        var total = await query.CountAsync();

        query = query.ApplyPagination(pageable);
        return new PagedResult<Invoice>
        {
            Content = await query.ToListAsync(),
            Total = total,
        };
    }
}