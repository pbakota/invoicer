using Invoicer.Data.Dao;
using Invoicer.Data.Utils;
using Invoicer.Models;

namespace Invoicer.App.Services;

public interface ITaxService : ICrudService<Tax>
{
    Task<Tax> NewTax();
    Task SaveTax(int taxId, Tax tax);
    Task CreateTax(Tax tax);
    Task DeleteTax(int taxId);
    Task<ICollection<Tax>> GetActiveTaxes();
}

public class TaxService(ITaxRepository taxRepository) : ITaxService
{
    private readonly ITaxRepository _taxRepository = taxRepository;

    public Pageable Pageable { get; } = new()
    {
        Page = 0,
        PageSize = 50,
        OrderBy = "Id asc",
        SearchTerm = null,
        Searchables = [
            nameof(Tax.ShortDescription),
            nameof(Tax.LongDescription),
        ],
    };

    public async Task CreateTax(Tax tax)
        => await _taxRepository.Create(tax);

    public async Task DeleteTax(int taxId)
        => await _taxRepository.DeleteById(taxId);

    public async Task<ICollection<Tax>> GetActiveTaxes()
        => await _taxRepository.GetActiveTaxes();

    public Task<PagedResult<Tax>> GetPagedData()
        => _taxRepository.FindAllPaged(Pageable);

    public async Task<Tax?> GetSingle(int taxId)
        => await _taxRepository.FindById(taxId);

    public Task<Tax> NewTax()
    {
        var model = new Tax { Active = true, };
        return Task.FromResult(model);
    }

    public async Task SaveTax(int taxId, Tax tax)
        => await _taxRepository.UpdateById(taxId, tax);

}