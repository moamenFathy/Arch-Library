// Infrastructure/Persistence/BookRepository.cs
using Library.Modules.Catalog.Application;
using Library.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;

namespace Library.Modules.Catalog.Infrastructure.Persistence;

public class BookRepository(CatalogDbContext db) : IBookRepository
{
    public async Task AddAsync(Book book, CancellationToken ct = default)
        => await db.Books.AddAsync(book, ct);

    public Task<Book?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Books.FirstOrDefaultAsync(b => b.Id == id, ct);

    public Task<bool> IsbnExistsAsync(string isbn, CancellationToken ct = default)
        => db.Books.AnyAsync(b => b.Isbn == isbn, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}