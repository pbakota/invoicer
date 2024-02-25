using Invoicer.App.Resources;
using Invoicer.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;

using Radzen.Blazor;

namespace Invoicer.App.Components.Pages.Invoices;

public partial class InvoiceItems : ComponentBase
{
    [Inject] IStringLocalizer<I18N> Loc { get; set; } = null!;

    [Parameter]
    public EventCallback<IList<InvoiceItem>> DataChanged { get; set; }

    [CascadingParameter]
    private IList<InvoiceItem> _items  { get; set;} = null!;

    private RadzenDataGrid<Models.InvoiceItem> _grid = null!;
    private int _count { get => _items.Count; }
    private int _idGen = 1;

    public async Task AddItem(Models.InvoiceItem item)
    {
        _items.Add(new InvoiceItem
        {
            Article = new Article
            {
                Id = item.Article.Id,
                Code = item.Article.Code,
                Name = item.Article.Name,
            },
            Price = item.Price,
            Qty = item.Qty,
            TaxRate = item.TaxRate,
            Rabat = item.Rabat,
            UOM = item.Article.UOM,
            Id = _idGen++,
        });
        await _grid.Reload();
        await DataChanged.InvokeAsync(_items);
    }

    public async Task RemoveItem(int itemId)
    {
        var item = _items.SingleOrDefault(x => x.Id == itemId);
        if (item is not null)
        {
            _items.Remove(item);
            await _grid.Reload();
            await DataChanged.InvokeAsync(_items);
        }
    }

    private async Task DeleteButtonClick(MouseEventArgs e, int id)
        => await RemoveItem(id);

    public IEnumerable<InvoiceItem> GetItems()
        => _items.AsReadOnly();
}