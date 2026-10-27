using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Library.Modules.Members.Domain;

namespace Library.Modules.Members.Infrastructure.Persistence.Configuration;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");
        
        builder.Property(m => m.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Ignore(m => m.FullName);

        builder.Property(m => m.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(m => m.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(m => m.MaxAllowedLoans)
            .IsRequired()
            .HasDefaultValue(5);

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.Property(m => m.UpdatedAt)
            .IsRequired(false);

        builder.Property(m => m.SuspensionReason)
            .HasMaxLength(500)
            .IsRequired(false);
    }
}