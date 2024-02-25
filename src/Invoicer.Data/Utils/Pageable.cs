namespace Invoicer.Data.Utils;

public record Pageable
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? OrderBy { get; set; }
    public string? SearchTerm { get; set; }
    public ICollection<string> Searchables = [];
}

public record PagedResult<T>
{
    public int Total { get; set; }
    public ICollection<T> Content { get; set; } = null!;
}
