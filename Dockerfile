FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/IMDB.Domain/IMDB.Domain.csproj src/IMDB.Domain/
COPY src/IMDB.Application/IMDB.Application.csproj src/IMDB.Application/
COPY src/IMDB.Infrastructure/IMDB.Infrastructure.csproj src/IMDB.Infrastructure/
COPY src/IMDB.WebAPI/IMDB.WebAPI.csproj src/IMDB.WebAPI/
RUN dotnet restore src/IMDB.WebAPI/IMDB.WebAPI.csproj

COPY src/ src/
RUN dotnet publish src/IMDB.WebAPI/IMDB.WebAPI.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "IMDB.WebAPI.dll"]
