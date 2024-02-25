using Invoicer.App.Services;
using Invoicer.Data.Dao;
using Invoicer.Data.Utils;
using Invoicer.Models;

namespace Invoicer.App;

public interface IPartnerService: ICrudService<Partner>
{
    Task DeletePartner(int partnerId);
    Task<Partner> NewPartner();
    Task CreatePartner(Partner partner);
    Task SavePartner(int partnerId, Partner partner);
    IEnumerable<string> GetPartnerTypes();
    Task<int> GenerateNextPartnerCode();
    Task<bool> IsCodeUnique(string code, int? partnerId);
}

public class PartnerService(IPartnerRepository partnerRepository) : IPartnerService
{
    private readonly IPartnerRepository _partnerRepository = partnerRepository;

    public Pageable Pageable { get; } = new()
    {
        Page = 0,
        PageSize = 50,
        OrderBy = "Id asc",
        SearchTerm = null,
        Searchables = [
            nameof(Partner.Name),
            nameof(Partner.Address),
            nameof(Partner.City),
            nameof(Partner.PostalCode),
            nameof(Partner.PIB),
        ],
    };

    public async Task DeletePartner(int partnerId)
        => await _partnerRepository.DeleteById(partnerId);

    public async Task<Partner?> GetSingle(int partnerId)
        => await _partnerRepository.FindById(partnerId);

    public async Task<PagedResult<Partner>> GetPagedData()
        => await _partnerRepository.FindAllPaged(Pageable);

    public async Task<int> GenerateNextPartnerCode()
        => await _partnerRepository.GetNextPartnerCode() + 1;

    public Task<Partner> NewPartner()
    {
        var model = new Partner();
        return Task.FromResult(model);
    }

    public async Task CreatePartner(Partner partner)
    {
        await _partnerRepository.Create(new Partner
        {
            Code = partner.Code,
            Name = partner.Name,
            Address = partner.Address,
            City = partner.City,
            PostalCode = partner.PostalCode,
            Phone = partner.Phone,
            Email = partner.Email,
            BankAccount = partner.BankAccount,
            BankName = partner.BankName,
            MaticniBroj = partner.MaticniBroj,
            PIB = partner.PIB,
            PartnerType = partner.PartnerType,
        });
    }

    public async Task SavePartner(int partnerId, Partner partner)
        => await _partnerRepository.UpdateById(partnerId, partner);

    public IEnumerable<string> GetPartnerTypes() => new string[] {
        PartnerType.CUSTOMER.ToString(),
    };
    public async Task<bool> IsCodeUnique(string code, int? partnerId)
        => await _partnerRepository.IsCodeUnique(code, partnerId);
}
