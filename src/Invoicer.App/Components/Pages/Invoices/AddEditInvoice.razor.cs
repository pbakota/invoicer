
using Invoicer.App.Constants;
using Invoicer.App.Services;
using Invoicer.App.Utils;
using Invoicer.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using Radzen;
using Radzen.Blazor;

namespace Invoicer.App.Components.Pages.Invoices;

public partial class AddEditInvoice : AddEditPage<Invoice>
{
    [Inject] private IInvoiceService InvoiceService { get; set; } = null!;
    [Inject] private IArticleService ArticleService { get; set; } = null!;
    [Inject] private IPartnerService PartnerService { get; set; } = null!;
    [Inject] private DialogService DialogService { get; set; } = null!;
    [Inject] private TooltipService TooltipService { get; set; } = null!;

    [SupplyParameterFromQuery(Name = "invoiceType")]
    public string? InvoiceTypeFromQuery { get; set; }

    private InvoiceType _invoiceType;
    private IEnumerable<string> _paymentMethods = null!;
    private string? _selectedPayment;
    private Partner? _selectedPartner;
    private ArticleEdit _articleEdit = null!;
    private InvoiceItems _invoiceItems = null!;
    private string? _subtotal = null!;
    private string? _avans = null!;
    private string? _total = null!;
    private string? _partnerInfo;
    private IList<InvoiceItem> _items = null!;
    private RadzenTemplateForm<Invoice> _form = null!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _items = new List<InvoiceItem>();
        _paymentMethods = InvoiceService.GetPaymentMethods();

        if (_editMode)
        {
            var invoice = await InvoiceService.GetSingle((int)_id!);
            if (invoice is null)
            {
                NotificationService.Error(Loc["Not found"]);
                NavigationManager.NavigateTo("/invoices");
                return;
            }

            _model = invoice;
            _items = _model.Items.ToList();

            _selectedPayment = _model.TypeOfPayment.ToString();

            CalculateSum(_items);
            UpdatePartnerInfo(_model.Partner);

            TopRowPageTitle = Loc[$"Invoice.Edit.{_invoiceType}"];
        }
        else
        {
            if (!Enum.TryParse(InvoiceTypeFromQuery?.ToUpper(), out _invoiceType))
            {
                NotificationService.Error(Loc["Invalid invoice type"]);
                NavigationManager.NavigateTo("/invoices");
                return;
            }

            _model = await InvoiceService.NewInvoice(_invoiceType);

            _selectedPayment = TypeOfPayment.VIRMANSKI.ToString();
            UpdateSums(0, 0, 0);

            TopRowPageTitle = Loc[$"Invoice.New.{_invoiceType}"];
        }
    }

    protected override void CancelClick()
        => NavigationManager.NavigateTo("/invoices");

    private async Task AddItemButtonClick(MouseEventArgs e)
    {
        var item = _articleEdit.GetItem();
        if (item.Article.Code is not null)
        {
            await _invoiceItems.AddItem(item);
        }
    }

    protected override async Task SaveClick()
    {
        if(await SaveInvoice() is not null)
        {
            if(_editMode)
            {
                NotificationService.Success(Loc["Text.Saved"]);
            }
            else
            {
                NotificationService.Success(Loc["Text.Created"]);
            }
            NavigationManager.NavigateTo("/invoices");
        }
        else
        {
            NotificationService.Error(Loc["Invoice.No.Items"]);
        }
    }

    private async Task<Invoice?> SaveInvoice()
    {
        if (_invoiceItems.GetItems().Count() == 0)
        {
            return null;
        }
        if (_editMode)
        {
            _model.PartnerId = _selectedPartner!.Id;
            return await InvoiceService.SaveInvoice((int)_id!, _model, _invoiceItems.GetItems());
        }
        else
        {
            _model.PartnerId = _selectedPartner!.Id;
            _model.TypeOfPayment = Enum.Parse<TypeOfPayment>(_selectedPayment!);
            return await InvoiceService.CreateInvoice(_model, _invoiceItems.GetItems());
        }
    }

    private async Task SelectArticleButtonClick(MouseEventArgs e)
    {
        var articleId = await DialogService.OpenAsync<ArticleSelect>(Loc["Text.Select.Article"], [],
            new DialogOptions() { Width = "80vw", Height = "80vh", Resizable = true, Draggable = true });

        if (articleId is null) return;

        var article = await ArticleService.GetSingle(articleId)!;
        _articleEdit.SetArticle(article);
    }

    private async Task SelectPartnerButtonClick(MouseEventArgs e)
    {
        var partnerId = await DialogService.OpenAsync<PartnerSelect>(Loc["Text.Select.Partner"], [],
            new DialogOptions() { Width = "80vw", Height = "80vh", Resizable = true, Draggable = true });

        if (partnerId is null) return;

        _selectedPartner = await PartnerService.GetSingle(partnerId)!;
        UpdatePartnerInfo(_selectedPartner);
    }

    private async Task SelectPrepayedButtonClick(MouseEventArgs e)
    {
        var prepayedId = await DialogService.OpenAsync<PrepayedSelect>(Loc["Text.Select.Prepayment"], [],
            new DialogOptions() { Width = "80vw", Height = "80vh", Resizable = true, Draggable = true });

        if(prepayedId is null) return;

        Invoice prepayed = await InvoiceService.GetSingle(prepayedId);

        _model.PrepaymentId = prepayedId;
        _model.PrepaymentSum = prepayed.InvoiceSum;

        CalculateSum(_items);
    }

    private async Task SaveAndPrint(MouseEventArgs e)
    {
        if(!_form.EditContext.Validate()) {
            NotificationService.Error(Loc["Form has errors!"]);
            return;
        }

        var invoice = await SaveInvoice();
        if(invoice is not null)
        {
            await InvoiceService.PrintInvoice(invoice.Id);
            if(_editMode)
            {
                NotificationService.Success(Loc["Text.Saved"]);
            }
            else
            {
                NotificationService.Success(Loc["Text.Created"]);
            }            
            NavigationManager.NavigateTo("/invoices");
        }
        else
        {
            NotificationService.Error(Loc["Invoice.No.Items"]);
        }
    }

    private void CalculateSum(IList<InvoiceItem> items)
    {
        var subtotal = items.Sum(c => PriceHelper.CalculateAmount(c));
        var total = subtotal - (double)(_model.PrepaymentSum ?? 0.0f);
        UpdateSums(subtotal, (double)(_model.PrepaymentSum ?? 0.0f), total);
    }

    private void UpdateSums(double subtotal, double avans, double total)
    {
        _subtotal = string.Format("{0:N2} {1}", subtotal, AppConstants.VALUTA);
        _avans = string.Format("{0:N2} {1}", avans, AppConstants.VALUTA);
        _total = string.Format("{0:N2} {1}", total, AppConstants.VALUTA);
    }

    private void UpdatePartnerInfo(Partner partner)
    {
        if (partner is not null)
        {
            _partnerInfo = $"{partner.Name}, {partner.Address}, {partner.City} {partner.PostalCode}";
        }
    }

    private void ShowAvansTooltip(ElementReference e)
        => TooltipService.Open(e, Loc["Invoice.PrepaymentInvoice.Select"], new TooltipOptions() { Duration = 10000 });
}
