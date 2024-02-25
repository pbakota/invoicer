using Invoicer.App.Extensions;
using Invoicer.App.Utils;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using Radzen;

namespace Invoicer.App.Components.Pages.Invoices;

public partial class PrepayedSelect : TableViewPage<Models.Invoice>
{
    [Inject] private IPrepayedService PrepayedService { get; set; } = null!;

    protected async override Task LoadData(LoadDataArgs args)
    {
        PrepayedService.Pageable.FromLoadArgs(args);

        var result = await PrepayedService.GetPagedData();
        _items = result.Content.ToList();
        _count = result.Total;
    }

    private async Task OnSearch(string? text)
    {
        PrepayedService.Pageable.SearchTerm = text;
        _grid!.CurrentPage = PrepayedService.Pageable.Page = 0;

        var result = await PrepayedService.GetPagedData();
        _items = result.Content.ToList();
        _count = result.Total;
   }

    public void SelectButtonClick(MouseEventArgs e, int? invoiceId)
        => DialogService.Close(invoiceId);    
}