using IMDB.Domain.Common;
using IMDB.Domain.Repositories;

namespace IMDB.Application.Common.Models;

public static class PagingExtensions
{
    public const int MaxPageSize = 100;

    public static (int Page, int PageSize) Normalize(int page, int pageSize)
    {
        return (Math.Max(page, 1), Math.Clamp(pageSize, 1, MaxPageSize));
    }

    public static async Task<PagedResult<TDto>> GetResultAsync<T, TDto>(
        this IRepository<T> repository,
        int? page,
        int? pageSize,
        Func<T, TDto> map,
        CancellationToken cancellationToken)
        where T : Entity, IAggregateRoot
    {
        if (page.HasValue && pageSize.HasValue)
        {
            var (currentPage, currentPageSize) = Normalize(page.Value, pageSize.Value);
            var (items, total) = await repository.GetPagedAsync(currentPage, currentPageSize, cancellationToken);

            return PagedResult<TDto>.Paged(items.Select(map).ToList(), total, currentPage, currentPageSize);
        }

        var all = await repository.GetAllAsync(cancellationToken);

        return PagedResult<TDto>.All(all.Select(map).ToList());
    }
}
