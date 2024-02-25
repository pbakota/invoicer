using Invoicer.Models;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Logging;

namespace Invoicer.Data.Dao;

public interface ITaxRepository : ICrudRepository<Tax>
{
    Task<ICollection<Tax>> GetActiveTaxes();
}

public class TaxRepository(IDbContextFactory<InvoicerContext> contextFactory, ILogger<Tax> logger)
    : CrudRepository<Tax>(contextFactory, logger), ITaxRepository
{
    public async Task<ICollection<Tax>> GetActiveTaxes()
    {
        using var db = _contextFactory.CreateDbContext();
        return await db.Taxes.AsNoTracking().Where(x => x.Active).ToListAsync();
    }
}