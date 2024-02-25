using Invoicer.App.Utils;
using Invoicer.Models;

using Microsoft.AspNetCore.Components;

using Radzen.Blazor;

namespace Invoicer.App.Components.Pages.Partners;

public partial class AddEditPartner : AddEditPage<Partner>
{
    [Inject] private IPartnerService PartnerService { get; set; } = null!;

    private RadzenTemplateForm<Partner> _form = null!;
    private bool _validCode = true;
    private IEnumerable<string> _partnerTypes = null!;
    private string? _selectedPartnerType;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _partnerTypes = PartnerService.GetPartnerTypes();

        if (_editMode)
        {
            TopRowPageTitle = Loc["Partner.Edit"];
            var partner = await PartnerService.GetSingle((int)_id!);
            if (partner is null)
            {
                NotificationService.Error(Loc["Not found"]);
                NavigationManager.NavigateTo("/partners");
                return;
            }
            _model = partner;
            _selectedPartnerType = partner.PartnerType.ToString();
        }
        else
        {
            _model = await PartnerService.NewPartner();
            var code = await PartnerService.GenerateNextPartnerCode();
            _model.Code = $"PAR{code:0000}";
            _selectedPartnerType = _partnerTypes.First();
            TopRowPageTitle = Loc["Partner.New"];
        }
    }

    protected override void CancelClick()
        => NavigationManager.NavigateTo("/partners");

    private async Task<bool> ValidateCode(string code, int? partnerId)
        => await PartnerService.IsCodeUnique(code, partnerId);

    protected override async Task SaveClick()
    {
        _validCode = await ValidateCode(_model.Code, _model.Id);
        if(!_form.EditContext.Validate())
        {
            OnInvalidSubmit();
            return;
        }

        if (_editMode)
        {
            _model.PartnerType = Enum.Parse<PartnerType>(_selectedPartnerType!);
            await PartnerService.SavePartner((int)_id!, _model);
            NotificationService.Success(Loc["Saved"]);
        }
        else
        {
            _model.PartnerType = Enum.Parse<PartnerType>(_selectedPartnerType!);
            await PartnerService.CreatePartner(_model);
            NotificationService.Success(Loc["Created"]);
        }
        NavigationManager.NavigateTo("/partners");
    }
}