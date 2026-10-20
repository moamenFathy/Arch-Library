using Library.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Modules.Catalog.Infrastructure.Persistence.Configuration;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Title).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Author).IsRequired().HasMaxLength(150);
        builder.Property(b => b.Isbn).IsRequired().HasMaxLength(20);
        builder.HasIndex(b => b.Isbn).IsUnique();
        
        builder.HasData(
            new 
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Title = "Clean Architecture",
                Author = "Robert C. Martin",
                Isbn = "978-0134494166"
            },
            new 
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Title = "Domain-Driven Design",
                Author = "Eric Evans",
                Isbn = "978-0321125217"
            },
            new 
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Title = "Refactoring",
                Author = "Martin Fowler",
                Isbn = "978-0201485677"
            }
        );
    }
}