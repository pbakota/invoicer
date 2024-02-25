using Invoicer.App.Extensions;
using Invoicer.App.Utils;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using Radzen;

namespace Invoicer.App.Components.Pages.Invoices;

public partial class PartnerSelect : TableViewPage<Models.Partner>
{
    [Inject] private IPartnerService PartnerService { get; set; } = null!;

    protected async override Task LoadData(LoadDataArgs args)
    {
        PartnerService.Pageable.FromLoadArgs(args);

        var result = await PartnerService.GetPagedData();
        _items = result.Content.ToList();
        _count = result.Total;
    }

    private async Task OnSearch(string? text)
    {
        PartnerService.Pageable.SearchTerm = text;
        _grid!.CurrentPage = PartnerService.Pageable.Page = 0;

        var result = await PartnerService.GetPagedData();
        _items = result.Content.ToList();
        _count = result.Total;
   }

    public void SelectButtonClick(MouseEventArgs e, int? partnerId)
        => DialogService.Close(partnerId);
}
