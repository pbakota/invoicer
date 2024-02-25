using Invoicer.App.Extensions;
using Invoicer.App.Utils;
using Invoicer.Data.Dao;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using Radzen;
using Radzen.Blazor;

namespace Invoicer.App.Components.Pages.Invoices;

public partial class Invoices : TableViewPage<Models.Invoice>
{
    [Inject] private IInvoiceRepository InvoiceRepository { get; set; } = null!;
    [Inject] private IInvoiceService InvoiceService { get; set; } = null!;

    protected async override Task LoadData(LoadDataArgs args)
    {
        InvoiceService.Pageable.FromLoadArgs(args);

        var result = await InvoiceService.GetPagedData();
        _items = result.Content.ToList();
        _count = result.Total;
    }

    private async Task OnSearch(string? text)
    {
        InvoiceService.Pageable.SearchTerm = text;
        _grid!.CurrentPage = InvoiceService.Pageable.Page = 0;

        var result = await InvoiceService.GetPagedData();
        _items = result.Content.ToList();
        _count = result.Total;
   }

    protected override void CreateButtonClick(MouseEventArgs e)
        => NavigationManager.NavigateTo("/invoices/edit");

    private void CreateInvoiceButtonClick(RadzenSplitButtonItem? item)
    {
        Console.WriteLine("{0}", item?.Value);
        switch (item?.Value)
        {
            default:
                NavigationManager.NavigateTo("/invoices/edit?invoiceType=normal");
                break;
            case "1":
                NavigationManager.NavigateTo("/invoices/edit?invoiceType=normal");
                break;
            case "2":
                NavigationManager.NavigateTo("/invoices/edit?invoiceType=proforma");
                break;
            case "3":
                NavigationManager.NavigateTo("/invoices/edit?invoiceType=prepayment");
                break;
        }
    }

    protected override void EditButtonClick(MouseEventArgs e, int id)
        => NavigationManager.NavigateTo($"/invoices/edit/{id}");

    protected async override void DeleteButtonClick(MouseEventArgs e, int id)
    {
        if (await ConfirmDelete())
        {
            await InvoiceRepository.DeleteById(id);
            NotificationService.Success(Loc["Deleted"]);
            NavigationManager.NavigateTo("/invoices", forceLoad: true);
        }
    }

    protected async void StornoButtonClick(MouseEventArgs e, int invoiceId)
    {
        await InvoiceService.StornoInvoice(invoiceId);
        await _grid!.RefreshDataAsync();
    }

    private async void PrintButtonClick(MouseEventArgs e, int invoiceId)
        => await InvoiceService.PrintInvoice(invoiceId);

    private static void RowRender(RowRenderEventArgs<Models.Invoice> args)
        => args.Attributes.Add("style", $"font-weight: {(args.Data.Storno ? "bold" : "normal")}; text-decoration: {(args.Data.Storno ? "line-through" : "")};");
}