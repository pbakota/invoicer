using Invoicer.App.Extensions;
using Invoicer.App.Utils;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using Radzen;

namespace Invoicer.App.Components.Pages.Partners;

public partial class Partners : TableViewPage<Models.Partner>
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

        var result = await PartnerService.GetPagedData();
        _items = result.Content.ToList();
        _count = result.Total;
    }

    protected override void CreateButtonClick(MouseEventArgs e)
        => NavigationManager.NavigateTo("/partners/edit");

    protected override void EditButtonClick(MouseEventArgs e, int partnerId)
        => NavigationManager.NavigateTo($"/partners/edit/{partnerId}");

    protected async override void DeleteButtonClick(MouseEventArgs e, int partnerId)
    {
        if (await ConfirmDelete())
        {
            await PartnerService.DeletePartner(partnerId);
            NotificationService.Success(Loc["Deleted"]);
            NavigationManager.NavigateTo("/partners", forceLoad: true);
        }
    }
}