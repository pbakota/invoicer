using Invoicer.Models;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Logging;

namespace Invoicer.Data.Dao;

public interface IPartnerRepository : ICrudRepository<Partner>
{
    Task<int> GetNextPartnerCode();
    Task<bool> IsCodeUnique(string code, int? partnerId);
}

public class PartnerRepository(IDbContextFactory<InvoicerContext> contextFactory, ILogger<Partner> logger)
    : CrudRepository<Partner>(contextFactory, logger), IPartnerRepository
{
    public async Task<int> GetNextPartnerCode()
    {
        using var db = _contextFactory.CreateDbContext();
        var code = await db.Set<Partner>().Where(f => f.Code.StartsWith("PAR")).MaxAsync(f => f.Code);
        if (code is null) return 0;
        var i = code.Length - 1;
        while (i >= 0 && code[i] >= '0' && code[i] <= '9') --i;
        return i < 0 ? 0 : int.TryParse(code[(i + 1)..], out int result) ? result : 0;
    }

    public async Task<bool> IsCodeUnique(string code, int? partnerId)
    {
        using var db = _contextFactory.CreateDbContext();
#pragma warning disable CA1862
        var result = !await db.Set<Partner>().AnyAsync(f => f.Code.ToLower() == code.ToLower() && (partnerId==null || partnerId != f.Id));
        return result;
    }
}