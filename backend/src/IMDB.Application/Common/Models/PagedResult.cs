namespace IMDB.Application.Common.Models;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int? Page, int? PageSize)
{
    public bool IsPaged => Page.HasValue;

    public static PagedResult<T> Paged(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        return new PagedResult<T>(items, totalCount, page, pageSize);
    }

    public static PagedResult<T> All(IReadOnlyList<T> items)
    {
        return new PagedResult<T>(items, items.Count, null, null);
    }
}
