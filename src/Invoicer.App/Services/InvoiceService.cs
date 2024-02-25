using Invoicer.App.Reports;
using Invoicer.App.Services;
using Invoicer.Data.Dao;
using Invoicer.Data.Utils;
using Invoicer.Models;

using Microsoft.JSInterop;

using QuestPDF.Fluent;

namespace Invoicer.App;

public interface IInvoiceService : ICrudService<Invoice>
{
    Task<Invoice> NewInvoice(InvoiceType invoiceType);
    Task<Invoice> CreateInvoice(Invoice invoice, IEnumerable<InvoiceItem> invoiceItems);
    Task<Invoice> SaveInvoice(int invoiceId, Invoice invoice, IEnumerable<InvoiceItem> invoiceItems);
    string GetInvoiceTypeId(InvoiceType invoiceType);
    Task<int> GenerateNextInvoiceId(InvoiceType invoiceType);
    IEnumerable<string> GetPaymentMethods();
    Task StornoInvoice(int invoiceId);
    Task PrintInvoice(int invoiceId);
}

public class InvoiceService(
    IInvoiceRepository invoiceRepository,
    IInvoiceReport invoiceReport,
    ISettingsService settingsService,
    IJSRuntime jSRuntime) : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;
    private readonly IInvoiceReport _invoiceReport = invoiceReport;
    private readonly ISettingsService _settingsService = settingsService;
    private readonly IJSRuntime _jSRuntime = jSRuntime;

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
        => await _invoiceRepository.FindAllPaged(Pageable);

    public async Task<Invoice> NewInvoice(InvoiceType invoiceType)
    {
        var settings = await _settingsService.LoadSettings();
        var invoiceNextId = await GenerateNextInvoiceId(invoiceType);
        var now = DateTime.Now;
        var invoiceTypeId = GetInvoiceTypeId(invoiceType);
        var invoice = new Invoice
        {
            InvoiceType = invoiceType,
            DateOfIssue = now,
            DateOfTraffic = now,
            Year = now.Year,
            NumberId = invoiceNextId,
            Number = $"{invoiceTypeId}-{invoiceNextId:00000}/{now.Year}",
            PlaceOfIssue = settings.PlaceOfIssue,
            PlaceOfTraffic = settings.PlaceOfTraffic,
            Partner = new Partner(),
            Text = settings.InvoiceInfoText,
        };

        return invoice;
    }

    public async Task<Invoice?> GetSingle(int invoiceId)
    {
        var invoice = await _invoiceRepository.FindById(invoiceId);
        if (invoice is null) return invoice;

        var model = new Invoice
        {
            Id = invoice.Id,
            Number = invoice.Number,
            NumberId = invoice.NumberId,
            InvoiceType = invoice.InvoiceType,
            DateOfIssue = invoice.DateOfIssue.ToLocalTime(),
            DateOfTraffic = invoice.DateOfTraffic.ToLocalTime(),
            Partner = invoice.Partner,
            PartnerId = invoice.PartnerId,
            PlaceOfIssue = invoice.PlaceOfIssue,
            PlaceOfTraffic = invoice.PlaceOfTraffic,
            Storno = invoice.Storno,
            Text = invoice.Text,
            TypeOfPayment = invoice.TypeOfPayment,
            Year = invoice.Year,
            Items = invoice.Items.ToList(),
            InvoiceSum = invoice.InvoiceSum,
            PrepaymentId = invoice.PrepaymentId,
            PrepaymentSum = invoice.PrepaymentSum,
        };

        return model;
    }

    public async Task<Invoice> SaveInvoice(int invoiceId, Invoice invoice, IEnumerable<InvoiceItem> invoiceItems)
    {
        var items = new List<InvoiceItem>();
        foreach (var item in invoiceItems)
        {
            items.Add(new InvoiceItem
            {
                Price = item.Price,
                ArticleId = item.Article.Id,
                Qty = item.Qty,
                Rabat = item.Rabat,
                TaxRate = item.TaxRate,
                UOM = item.UOM,
            });
        }
        var invoiceTypeId = GetInvoiceTypeId(invoice.InvoiceType);
        invoice.InvoiceSum = (float)Math.Round(items.Sum(PriceHelper.CalculateAmount), 2);
        invoice.Number = $"{invoiceTypeId}-{invoice.NumberId:00000}/{invoice.DateOfIssue.Year}";
        invoice.Year = invoice.DateOfIssue.Year;
        invoice.DateOfIssue = invoice.DateOfIssue.ToUniversalTime();
        invoice.DateOfTraffic = invoice.DateOfTraffic.ToUniversalTime();
        invoice.Items = items;
        await _invoiceRepository.UpdateById(invoiceId, invoice);

        return invoice;
    }

    public string GetInvoiceTypeId(InvoiceType invoiceType) => invoiceType switch
    {
        InvoiceType.NORMAL => "RO",
        InvoiceType.PROFORMA => "PR",
        InvoiceType.PREPAYMENT => "AV",
        _ => ""
    };

    public async Task<int> GenerateNextInvoiceId(InvoiceType invoiceType)
        => await _invoiceRepository.GetNextInvoiceId(invoiceType) + 1;

    public async Task<Invoice> CreateInvoice(Invoice invoice, IEnumerable<InvoiceItem> invoiceItems)
    {
        var items = new List<InvoiceItem>();
        foreach (var item in invoiceItems)
        {
            items.Add(new InvoiceItem
            {
                Price = item.Price,
                ArticleId = item.Article.Id,
                Qty = item.Qty,
                Rabat = item.Rabat,
                TaxRate = item.TaxRate,
                UOM = item.UOM,
            });
        }
        var invoiceTypeId = GetInvoiceTypeId(invoice.InvoiceType);
        var newInvoice = new Models.Invoice
        {
            InvoiceType = invoice.InvoiceType,
            InvoiceSum = (float)Math.Round(items.Sum(PriceHelper.CalculateAmount), 2),
            NumberId = invoice.NumberId,
            Number = $"{invoiceTypeId}-{invoice.NumberId:00000}/{invoice.DateOfIssue.Year}",
            DateOfIssue = invoice.DateOfIssue.ToUniversalTime(),
            DateOfTraffic = invoice.DateOfTraffic.ToUniversalTime(),
            PlaceOfIssue = invoice.PlaceOfIssue,
            PlaceOfTraffic = invoice.PlaceOfTraffic,
            PartnerId = invoice.PartnerId,
            Storno = false,
            TypeOfPayment = invoice.TypeOfPayment,
            Year = invoice.DateOfIssue.ToUniversalTime().Year,
            Text = invoice.Text,
            Items = items,
            PrepaymentId = invoice.PrepaymentId,
            PrepaymentSum = invoice.PrepaymentSum
        };
        await _invoiceRepository.Create(newInvoice);
        return newInvoice;
    }

    public IEnumerable<string> GetPaymentMethods() => new string[] {
            TypeOfPayment.GOTOVINA.ToString(),
            TypeOfPayment.VIRMANSKI.ToString(),
            TypeOfPayment.KARTICA.ToString(),
        };

    public async Task PrintInvoice(int invoiceId)
    {
        var invoice = await _invoiceRepository.FindById(invoiceId);
        var settings = await _settingsService.LoadSettings();
        if (invoice is not null)
        {
            #if ELECTRON_APP
                await Task.Run(() => _invoiceReport.CreateDocument(invoice, settings).GeneratePdfAndShow());
            #else
                using var filestream = new MemoryStream(_invoiceReport.CreateDocument(invoice, settings).GeneratePdf());
                using var streamRef = new DotNetStreamReference(stream: filestream);
                await _jSRuntime.InvokeVoidAsync("downloadPdfFromStream", streamRef);
            #endif
        }
    }

    public async Task StornoInvoice(int invoiceId)
        => await _invoiceRepository.StornoInvoice(invoiceId);
}
