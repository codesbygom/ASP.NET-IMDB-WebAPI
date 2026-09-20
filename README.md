# IMDB Web API

An IMDb-style REST API for movies, series, seasons, episodes, people, cast, comments and ratings. Built with **ASP.NET Core 8**, **EF Core (SQL Server)**, **ASP.NET Core Identity + JWT**, **MediatR** and structured with **Clean Architecture** and tactical **Domain-Driven Design**.

## Architecture

```
src/
  IMDB.Domain          Aggregates, entities, value objects, domain exceptions, repository contracts.
                       No package dependencies.
  IMDB.Application     Use cases as MediatR commands and queries, DTOs, mappings, pipeline behaviors,
                       abstractions (identity, tokens, current user). Depends on Domain only.
  IMDB.Infrastructure  EF Core DbContext and Fluent API configurations, migrations, repositories,
                       ASP.NET Core Identity, JWT token service.
  IMDB.WebAPI          Thin controllers, request contracts, exception middleware, composition root.
tests/
  IMDB.Tests           Domain unit tests and handler tests running against an in-memory SQLite database.
```

Dependencies point inward: `WebAPI -> Infrastructure -> Application -> Domain`.

### Domain model

- **Aggregate roots:** `Movie`, `Series`, `Genre`, `Person`, `Cast`, `Comment`, `Rate`. Each has its own repository; a single `IUnitOfWork` commits changes.
- **`Series` owns its `Season`s, and each `Season` owns its `Episode`s.** They are created, changed and removed only through the series (`AddSeason`, `AddEpisode`, ...), which enforces unique season and episode numbers.
- **Value objects:** `Rating` (0 to 10) and `ImdbId`, converted to primitives in EF configuration.
- **Invariants live in the entities:** private setters, factory methods, guard clauses, and ownership rules (only the author can edit a comment or a rate).
- Other aggregates are referenced by id, and users are referenced by `UserId` only, so the Domain has no dependency on ASP.NET Core Identity.

### Application

Every use case is a MediatR request with its own handler, for example `CreateMovieCommand`, `GetGenresQuery`, `AddCommentCommand`. A `LoggingBehavior` pipeline behavior wraps every request. Usernames are resolved through `IIdentityService`, which keeps Identity out of the domain model.

## Run locally

Start SQL Server and the API with Docker:

```bash
cp .env.example .env
docker compose up --build
```

Or run the API directly (LocalDB by default):

```bash
cd src/IMDB.WebAPI
dotnet user-secrets set "Jwt:SigningKey" "<random string, at least 32 characters>"
dotnet user-secrets set "Database:MigrateOnStartup" "true"
dotnet run
```

Swagger UI is available at `/swagger`. To create an initial admin, set `Seed:AdminUserName`, `Seed:AdminEmail` and `Seed:AdminPassword` (user-secrets or environment variables). Nothing is seeded when they are empty.

## Tests

```bash
dotnet test
```

## Main endpoints

| Resource | Routes |
| --- | --- |
| Account | `POST /api/account/register`, `POST /api/account/login` |
| Genre, Movie, Series, Season, Episode, Person | `GET` list and detail, `POST`, `PUT`, `DELETE` (admin) |
| Cast | `GET /api/cast`, `GET /api/cast/media/{mediaId}`, `POST`, `PUT`, `DELETE` |
| Comment, Rate | `GET /api/{comment\|rate}/media/{mediaId}`, `POST`, `PUT`, `DELETE` (authenticated, author only) |

List endpoints for genres, people and cast accept `page` and `pageSize`.
