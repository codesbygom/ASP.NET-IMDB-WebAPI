using IMDB.Domain.Entities;
using IMDB.Domain.ValueObjects;
using IMDB.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMDB.Infrastructure.Persistence.Configurations;

public sealed class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable("Genres");
        builder.Property(g => g.Title).HasMaxLength(50).IsRequired();
    }
}

public sealed class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("People");

        builder.Property(p => p.ImdbId)
            .HasConversion(v => v.Value, v => ImdbId.From(v))
            .HasMaxLength(ImdbId.MaxLength)
            .IsRequired();

        builder.Property(p => p.FullName).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Bio).HasMaxLength(2000).IsRequired();
        builder.Property(p => p.PhotoUrl).HasMaxLength(500).IsRequired();
    }
}

public sealed class CastConfiguration : IEntityTypeConfiguration<Cast>
{
    public void Configure(EntityTypeBuilder<Cast> builder)
    {
        builder.ToTable("Cast");

        builder.Property(c => c.Role).HasConversion<string>().HasMaxLength(50);

        builder.HasOne(c => c.Person)
            .WithMany()
            .HasForeignKey(c => c.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Media>()
            .WithMany()
            .HasForeignKey(c => c.MediaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");

        builder.Property(c => c.Text).HasMaxLength(1000).IsRequired();

        builder.HasOne<Media>()
            .WithMany()
            .HasForeignKey(c => c.MediaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class RateConfiguration : IEntityTypeConfiguration<Rate>
{
    public void Configure(EntityTypeBuilder<Rate> builder)
    {
        builder.ToTable("Rates");

        builder.Property(r => r.Score).HasConversion<double>();

        builder.HasOne<Media>()
            .WithMany()
            .HasForeignKey(r => r.MediaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
