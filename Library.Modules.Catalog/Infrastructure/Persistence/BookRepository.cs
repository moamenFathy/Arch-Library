// Infrastructure/Persistence/BookRepository.cs
using Library.Modules.Catalog.Application;
using Library.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;

namespace Library.Modules.Catalog.Infrastructure.Persistence;

public class BookRepository(CatalogDbContext context) : IBookRepository
{
    public async Task AddAsync(Book book, CancellationToken ct = default)
        => await context.Books.AddAsync(book, ct);

    public Task<Book?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => context.Books.FirstOrDefaultAsync(b => b.Id == id, ct);

    public Task<bool> IsbnExistsAsync(string isbn, CancellationToken ct = default)
        => context.Books.AnyAsync(b => b.Isbn == isbn, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => context.SaveChangesAsync(ct);
}