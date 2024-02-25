using Invoicer.App.Resources;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Invoicer.App.Components.Pages.Invoices;

public partial class ArticleEdit : ComponentBase
{
    [Inject] private IStringLocalizer<I18N> Loc { get; set; } = null!;

    private Models.InvoiceItem _model = null!;

    protected override Task OnInitializedAsync()
    {
        _model = new Models.InvoiceItem
        {
            Article = new()
        };

        return Task.CompletedTask;
    }

    public void SetArticle(Models.Article article)
    {
        _model.Id = article.Id;
        _model.Article = article;
        _model.Price = article.Price;
        _model.UOM = article.UOM;
        _model.Qty = 1;
        _model.TaxRate = article.Tax.Rate;

        StateHasChanged();
    }

    public Models.InvoiceItem GetItem() => _model;
}