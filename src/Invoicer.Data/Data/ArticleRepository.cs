using Invoicer.Data.Extensions;

using Invoicer.Data.Utils;
using Invoicer.Models;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Logging;

namespace Invoicer.Data.Dao;

public interface IArticleRepository : ICrudRepository<Article>
{
    Task<int> GetNextArticleCode();
    Task<bool> IsCodeUnique(string code, int? articleId);
}

public class ArticleRepository(IDbContextFactory<InvoicerContext> contextFactory, ILogger<Article> logger)
    : CrudRepository<Article>(contextFactory, logger), IArticleRepository
{
    public override async Task<PagedResult<Article>> FindAllPaged(Pageable pageable)
    {
        using var db = _contextFactory.CreateDbContext();
        var query = db.Set<Article>().AsNoTracking().Include(x => x.Tax).ApplySearchAndOrder(pageable);
        var total = await query.CountAsync();

        query = query.ApplyPagination(pageable);
        return new PagedResult<Article>
        {
            Content = await query.ToListAsync(),
            Total = total,
        };
    }

    public override async Task<Article?> FindById(int entityId)
    {
        using var db = _contextFactory.CreateDbContext();
        return await db.Set<Article>().Include(x => x.Tax).SingleOrDefaultAsync(x => x.Id == entityId);
    }

    public async Task<int> GetNextArticleCode()
    {
        using var db = _contextFactory.CreateDbContext();
        var code = await db.Set<Article>().Where(f => f.Code.StartsWith("ART")).MaxAsync(f => f.Code);
        if (code is null) return 0;
        var i = code.Length - 1;
        while (i >= 0 && code[i] >= '0' && code[i] <= '9') --i;
        return i < 0 ? 0 : int.TryParse(code[(i + 1)..], out int result) ? result : 0;
    }

    public async Task<bool> IsCodeUnique(string code, int? articleId)
    {
        using var db = _contextFactory.CreateDbContext();
#pragma warning disable CA1862
        var result = !await db.Set<Article>().AnyAsync(f => f.Code.ToLower() == code.ToLower() && (articleId==null || articleId != f.Id));
        return result;
    }
}