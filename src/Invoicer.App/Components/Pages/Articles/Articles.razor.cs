using Invoicer.App.Extensions;
using Invoicer.App.Services;
using Invoicer.App.Utils;
using Invoicer.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using Radzen;

namespace Invoicer.App.Components.Pages.Articles;

public partial class Articles : TableViewPage<Article>
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
        // Reset grid page
        _grid!.CurrentPage = ArticleService.Pageable.Page = 0;

        var result = await ArticleService.GetPagedData();
        _items = result.Content.ToList();
        _count = result.Total;
    }

    protected override void CreateButtonClick(MouseEventArgs e)
        => NavigationManager.NavigateTo("/articles/edit");

    protected override void EditButtonClick(MouseEventArgs e, int articleId)
        => NavigationManager.NavigateTo($"/articles/edit/{articleId}");

    protected async override void DeleteButtonClick(MouseEventArgs e, int articleId)
    {
        if (await ConfirmDelete())
        {
            await ArticleService.DeleteArticle(articleId);
            NotificationService.Success(Loc["Deleted"]);
            NavigationManager.NavigateTo("/articles", forceLoad: true);
        }
    }
}