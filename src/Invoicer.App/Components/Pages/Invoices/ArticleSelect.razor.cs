using Invoicer.App.Extensions;
using Invoicer.App.Services;
using Invoicer.App.Utils;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using Radzen;

namespace Invoicer.App.Components.Pages.Invoices;

public partial class ArticleSelect : TableViewPage<Models.Article>
{
    [Inject] private IArticleService ArticleService { get; set; } = null!;

    protected async override Task LoadData(LoadDataArgs args)
    {
        ArticleService.Pageable.FromLoadArgs(args);

        var result = await ArticleService.GetPagedData();
        _items = result.Content.ToList();
        _count = result.Total;
    }

    private async Task OnSearch(string? text)
    {
        ArticleService.Pageable.SearchTerm = text;
        _grid!.CurrentPage = ArticleService.Pageable.Page = 0;

        var result = await ArticleService.GetPagedData();
        _items = result.Content.ToList();
        _count = result.Total;
    }

    private void SelectButtonClick(MouseEventArgs e, int id)
        => DialogService.Close(id);
}