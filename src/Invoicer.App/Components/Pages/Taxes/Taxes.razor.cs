using Invoicer.App.Extensions;
using Invoicer.App.Services;
using Invoicer.App.Utils;
using Invoicer.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using Radzen;

namespace Invoicer.App.Components.Pages.Taxes;

public partial class Taxes : TableViewPage<Tax>
{
    [Inject] private ITaxService TaxService { get; set; } = null!;

    protected async override Task LoadData(LoadDataArgs args)
    {
        TaxService.Pageable.FromLoadArgs(args);
        
        var result = await TaxService.GetPagedData();

        _items = result.Content.ToList();
        _count = result.Total;
    }

    private async Task OnSearch(string? text)
    {
        TaxService.Pageable.SearchTerm = text;
        
        var result = await TaxService.GetPagedData();
        _items = result.Content.ToList();
        _count = result.Total;
    }

    protected override void CreateButtonClick(MouseEventArgs e)
        => NavigationManager.NavigateTo("/taxes/edit");

    protected override void EditButtonClick(MouseEventArgs e, int id)
        => NavigationManager.NavigateTo($"/taxes/edit/{id}");

    protected async override void DeleteButtonClick(MouseEventArgs e, int id)
    {
        if (await ConfirmDelete())
        {
            await TaxService.DeleteTax(id);
            NotificationService.Success(Loc["Deleted"]);
            NavigationManager.NavigateTo("/taxes", forceLoad: true);
        }
    }
}