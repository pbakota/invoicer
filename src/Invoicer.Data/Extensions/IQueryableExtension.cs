using System.Linq.Dynamic.Core;

using Invoicer.Data.Utils;
using Invoicer.Models;

namespace Invoicer.Data.Extensions;

public static class IQueryableExtension
{
    public static IQueryable<TEntity> ApplySearchAndOrder<TEntity>(this IQueryable<TEntity> query, Pageable pageable)
    {
        if (!string.IsNullOrEmpty(pageable.SearchTerm))
        {
            var conditions = new List<string>();
            foreach(var field in pageable.Searchables) {                
                conditions.Add($"{field}.ToLower().Contains(\"{pageable.SearchTerm.ToLower()}\")");
            }
            query = query.Where(string.Join(" || ", conditions));
        }
        if (!string.IsNullOrEmpty(pageable.OrderBy))
        {
            query = query.OrderBy(pageable.OrderBy);
        }
        return query;
    }

    private static int CalcPageOffset(Pageable pageable)
        => pageable.Page < 0 ? 0 : (pageable.Page * pageable.PageSize);

    public static IQueryable<TEntity> ApplyPagination<TEntity>(this IQueryable<TEntity> query, Pageable pageable)
    {
        var pageOffset = CalcPageOffset(pageable);
        query = query.Skip(pageOffset).Take(pageable.PageSize);
        return query;
    }
}