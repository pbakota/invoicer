using Invoicer.Data.Dao;
using Invoicer.Data.Utils;
using Invoicer.Models;

namespace Invoicer.App;

public interface IPrepayedService
{
    Pageable Pageable { get; }
    Task<PagedResult<Invoice>> GetPagedData();
}

public class PrepayedService(IInvoiceRepository invoiceRepository) : IPrepayedService
{
    private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;

    public Pageable Pageable { get; } = new()
    {
        Page = 0,
        PageSize = 50,
        OrderBy = "Id asc",
        SearchTerm = null,
        Searchables = [
            nameof(Invoice.Number),
            "Partner.Name",
        ],
    };

    public async Task<PagedResult<Invoice>> GetPagedData()
        => await _invoiceRepository.FindAllPrepayedPaged(Pageable);

}

