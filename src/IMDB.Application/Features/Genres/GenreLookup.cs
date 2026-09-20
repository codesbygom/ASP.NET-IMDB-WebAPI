using IMDB.Application.Common.Exceptions;
using IMDB.Domain.Entities;
using IMDB.Domain.Repositories;

namespace IMDB.Application.Features.Genres;

internal static class GenreLookup
{
    public static async Task<IReadOnlyList<Genre>> ResolveAsync(
        IGenreRepository genres,
        IReadOnlyList<int> genreIds,
        CancellationToken cancellationToken)
    {
        var distinctIds = genreIds.Distinct().ToList();
        var found = await genres.GetByIdsAsync(distinctIds, cancellationToken);

        if (found.Count != distinctIds.Count)
            throw new NotFoundException("Invalid Genre ID");

        return found;
    }
}
