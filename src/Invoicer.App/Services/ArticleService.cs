using Invoicer.Data.Dao;
using Invoicer.Data.Utils;
using Invoicer.Models;

namespace Invoicer.App.Services;

public interface IArticleService : ICrudService<Article>
{
    Task<Article> NewArticle();
    Task SaveArticle(int articleId, Article article);
    Task CreateArticle(Article article);
    Task DeleteArticle(int articleId);
    Task<int> GenerateNextArticleCode();
    Task<bool> IsCodeUnique(string code, int? articleId);
}

public class ArticleService(IArticleRepository articleRepository) : IArticleService
{
    private readonly IArticleRepository _articleRepository = articleRepository;

    public Pageable Pageable { get; } = new()
    {
        Page = 0,
        PageSize = 50,
        OrderBy = "Id asc",
        SearchTerm = null,
        Searchables = [
            nameof(Article.Name),
            nameof(Article.Code),
        ],
    };

    public async Task<Article?> GetSingle(int articleId)
        => await _articleRepository.FindById(articleId);

    public async Task<PagedResult<Article>> GetPagedData()
        => await _articleRepository.FindAllPaged(Pageable);

    public Task<Article> NewArticle()
    {
        var model = new Article();
        return Task.FromResult(model);
    }

    public async Task SaveArticle(int articleId, Article article)
        => await _articleRepository.UpdateById(articleId, article);

    public async Task CreateArticle(Article article)
        => await _articleRepository.Create(article);

    public async Task DeleteArticle(int articleId)
        => await _articleRepository.DeleteById(articleId);

    public async Task<int> GenerateNextArticleCode()
        => await _articleRepository.GetNextArticleCode() + 1;

    public async Task<bool> IsCodeUnique(string code, int? articleId)
        => await _articleRepository.IsCodeUnique(code, articleId);
}
