using Invoicer.App.Services;
using Invoicer.App.Utils;
using Invoicer.Models;

using Microsoft.AspNetCore.Components;

namespace Invoicer.App.Components.Pages.Taxes;

public partial class AddEditTax : AddEditPage<Models.Tax>
{
    [Inject] private ITaxService TaxService { get; set; } = null!;

    protected async override Task OnInitializedAsync()
    {
        // await Task.Delay(2000);

        await base.OnInitializedAsync();
        if (_editMode)
        {
            TopRowPageTitle = Loc["Tax.Edit"];
            var tax = await TaxService.GetSingle((int)_id!);
            if (tax is null)
            {
                NotificationService.Error(Loc["Tax not found"]);
                NavigationManager.NavigateTo("/taxes");
                return;
            }
            _model = tax;
        }
        else
        {
            _model = await TaxService.NewTax();
            TopRowPageTitle = Loc["Tax.New"];
        }
    }

    protected override void CancelClick()
        => NavigationManager.NavigateTo("/taxes");

    protected override async Task SaveClick()
    {
        if (_editMode)
        {
            await TaxService.SaveTax((int)_id!, _model);
            NotificationService.Success(Loc["Text.Saved"]);
        }
        else
        {
            await TaxService.CreateTax(new Tax
            {
                LongDescription = _model.LongDescription,
                ShortDescription = _model.ShortDescription,
                Rate = _model.Rate,
                Active = _model.Active,
            });
            NotificationService.Success(Loc["Text.Created"]);
        }

        NavigationManager.NavigateTo("/taxes");
    }
}