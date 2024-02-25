using Invoicer.App.Services;
using Invoicer.App.Utils;
using Invoicer.Models;

using Microsoft.AspNetCore.Components;

using Radzen.Blazor;

namespace Invoicer.App.Components.Pages.Articles;

public partial class AddEditArticle : AddEditPage<Article>
{
    [Inject] private IArticleService ArticleService { get; set; } = null!;
    [Inject] private ITaxService TaxService { get; set; } = null!;

    private RadzenTemplateForm<Article> _form = null!;
    private bool _validCode = true;
    private ICollection<Tax> _activeTaxes = null!;
    private int? _selectedTaxId;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _activeTaxes = await TaxService.GetActiveTaxes();

        if (_editMode)
        {
            TopRowPageTitle = Loc["Article.Edit"];
            var article = await ArticleService.GetSingle((int)_id!);
            if (article is null)
            {
                NotificationService.Error(Loc["Article not found"]);
                NavigationManager.NavigateTo("/articles");
                return;
            }
            _model = article;
            _selectedTaxId = _model.TaxId;
        }
        else
        {
            _model = await ArticleService.NewArticle();
            var code = await ArticleService.GenerateNextArticleCode();
            _model.Code = $"ART{code:0000}";
            _selectedTaxId = null;
            TopRowPageTitle = Loc["Article.New"];
        }
    }

    private async Task<bool> ValidateCode(string code, int articleId)
        => await ArticleService.IsCodeUnique(code, articleId);

    protected override void CancelClick()
        => NavigationManager.NavigateTo("/articles");

    protected override async Task SaveClick()
    {
        // NOTE: A workaround for async custom validator
        _validCode = await ValidateCode(_model.Code, _model.Id);
        if(!_form.EditContext.Validate()) {
            OnInvalidSubmit();
            return;
        }

        var tax = _activeTaxes.First(x => x.Id == (int)_selectedTaxId!);
        if (_editMode)
        {
            _model.TaxId = (int)_selectedTaxId!;
            await ArticleService.SaveArticle((int)_id!, _model);
            NotificationService.Success(Loc["Text.Saved"]);
        }
        else
        {
            await ArticleService.CreateArticle(new Article
            {
                Code = _model.Code,
                Name = _model.Name,
                UOM = _model.UOM,
                Price = _model.Price,
                TaxId = (int)_selectedTaxId!,
            });
            NotificationService.Success(Loc["Text.Created"]);
        }
        NavigationManager.NavigateTo("/articles");
    }
}