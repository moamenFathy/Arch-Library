using Library.Modules.Catalog.Application.Books;
using Microsoft.EntityFrameworkCore;

namespace Library.Modules.Catalog.Infrastructure.Persistence;

public class BookQueries(CatalogDbContext context) : IBookQueries
{
    // get all
    public async Task<List<BookDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Books
            .AsNoTracking()    
            .Select(b => new BookDto(b.Id, b.Title, b.Author, b.Isbn, b.CategoryId, b.Category.Name))
            .ToListAsync(ct);
    }
    
    // get by id with no tracking
    public async Task<BookDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Books
            .AsNoTracking()
            .Where(b => b.Id == id)
            .Select(b => new BookDto(b.Id, b.Title, b.Author, b.Isbn, b.CategoryId, b.Category.Name))
            .FirstOrDefaultAsync(ct);
    }
}
