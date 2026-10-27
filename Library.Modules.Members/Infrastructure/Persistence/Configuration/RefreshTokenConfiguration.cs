using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Library.Modules.Members.Domain;

namespace Library.Modules.Members.Infrastructure.Persistence.Configuration;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.MemberId)
            .IsRequired();

        builder.HasIndex(t => t.MemberId);

        builder.Property(t => t.Token)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(t => t.Token)
            .IsUnique();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.ExpiresAt)
            .IsRequired();

        builder.Property(t => t.RevokedAt)
            .IsRequired(false);

        builder.Ignore(t => t.IsExpired);
        builder.Ignore(t => t.IsRevoked);
        builder.Ignore(t => t.IsActive);

        // Foreign key relation at DB level without polluting domain models
        builder.HasOne<Member>()
            .WithMany()
            .HasForeignKey(t => t.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}