using Invoicer.Data;
using Invoicer.Models;

using Microsoft.EntityFrameworkCore;

namespace Invoicer.App.Services;

public record PlainStats
{
    public int NumberOfPartners { get; set; }
    public int NumberOfArticles { get; set; }
    public int NumberOfInvoices { get; set; }
}
public class QuarterlyData
{
    public int Year { get; set; }
    public string Quarter { get; set; } = null!;
    public double Revenue { get; set; }
}

public interface IDashboardService
{
    Task<PlainStats> GetPlainStats();
    Task<IList<QuarterlyData>> GetRevenue();
}

public class DashboardService(IDbContextFactory<InvoicerContext> contextFactory) : IDashboardService
{
    private readonly IDbContextFactory<InvoicerContext> _contextFactory = contextFactory;


    public async Task<PlainStats> GetPlainStats()
    {
        using var db = _contextFactory.CreateDbContext();

        var numberOfPartners = await db.Set<Partner>().CountAsync();
        var numberOfArticles = await db.Set<Article>().CountAsync();
        var numberOfInvoices = await db.Set<Invoice>().CountAsync();
        return new PlainStats
        {
            NumberOfArticles = numberOfArticles,
            NumberOfPartners = numberOfPartners,
            NumberOfInvoices = numberOfInvoices,
        };
    }

    public async Task<IList<QuarterlyData>> GetRevenue()
    {
        using var db = _contextFactory.CreateDbContext();

        var result = new List<QuarterlyData>();
        var startYear = DateTime.Now.Year - 1;
        var revenue = await db.Invoices.Where(x => x.DateOfIssue.Year >= startYear && x.InvoiceType != InvoiceType.PROFORMA && !x.Storno)
            .Select(x => new { y = x.DateOfIssue.Year, q = (x.DateOfIssue.Month - 1) / 3 + 1, s = x.InvoiceSum })
            .ToListAsync();

        for (var y = startYear; y <= startYear + 1; ++y)
        {
            for (var q = 1; q <= 4; ++q)
            {
                var s = revenue.Where(x => x.q == q && x.y == y).Sum(x => x.s);
                result.Add(new QuarterlyData
                {
                    Quarter = "Q" + q,
                    Year = y,
                    Revenue = s,
                });
            }
        }

        return result;
    }
}
