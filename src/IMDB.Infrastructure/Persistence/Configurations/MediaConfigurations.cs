using IMDB.Domain.Entities;
using IMDB.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMDB.Infrastructure.Persistence.Configurations;

public sealed class MediaConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.ToTable("Media");
        builder.UseTptMappingStrategy();

        builder.Property(m => m.ImdbId)
            .HasConversion(v => v.Value, v => ImdbId.From(v))
            .HasMaxLength(ImdbId.MaxLength)
            .IsRequired();

        builder.Property(m => m.Title).HasMaxLength(200).IsRequired();
        builder.Property(m => m.Description).HasMaxLength(2000).IsRequired();
        builder.Property(m => m.PosterUrl).HasMaxLength(500).IsRequired();

        builder.Property(m => m.Rate)
            .HasConversion(v => v.Value, v => Rating.From(v))
            .IsRequired();
    }
}

public sealed class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.ToTable("Movies");

        builder.HasMany(m => m.Genres)
            .WithMany()
            .UsingEntity(j => j.ToTable("MovieGenres"));

        builder.Navigation(m => m.Genres).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public sealed class SeriesConfiguration : IEntityTypeConfiguration<Series>
{
    public void Configure(EntityTypeBuilder<Series> builder)
    {
        builder.ToTable("Series");

        builder.HasMany(s => s.Genres)
            .WithMany()
            .UsingEntity(j => j.ToTable("SeriesGenres"));

        builder.HasMany(s => s.Seasons)
            .WithOne()
            .HasForeignKey(season => season.SeriesId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(s => s.Genres).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(s => s.Seasons).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public sealed class SeasonConfiguration : IEntityTypeConfiguration<Season>
{
    public void Configure(EntityTypeBuilder<Season> builder)
    {
        builder.ToTable("Seasons");

        builder.HasMany(s => s.Episodes)
            .WithOne()
            .HasForeignKey(e => e.SeasonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(s => s.Episodes).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public sealed class EpisodeConfiguration : IEntityTypeConfiguration<Episode>
{
    public void Configure(EntityTypeBuilder<Episode> builder)
    {
        builder.ToTable("Episodes");
    }
}
