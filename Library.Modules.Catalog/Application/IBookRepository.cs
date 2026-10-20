// Application/Books/IBookRepository.cs
using Library.Modules.Catalog.Domain;

namespace Library.Modules.Catalog.Application;

public interface IBookRepository
{
    Task AddAsync(Book book, CancellationToken ct = default);
    Task<Book?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> IsbnExistsAsync(string isbn, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}