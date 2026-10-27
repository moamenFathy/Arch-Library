using Library.Modules.Catalog.Application.Books;
using Library.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace Library.Modules.Catalog.Infrastructure.Persistence;

public class BookQueries(CatalogDbContext context) : IBookQueries
{
    public async Task<PagedResult<BookDto>> GetAllAsync(BookFilterParams filter, CancellationToken ct = default)
    {
        var query = context.Books.AsNoTracking();

        // 1. فلترة البحث العام (العنوان أو المؤلف أو ISBN)
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            query = query.Where(b => 
                EF.Functions.ILike(b.Title, $"%{term}%") || 
                EF.Functions.ILike(b.Author, $"%{term}%") ||
                b.Isbn.Contains(term));
        }

        // 2. فلترة محددة بالمؤلف
        if (!string.IsNullOrWhiteSpace(filter.Author))
        {
            var author = filter.Author.Trim();
            query = query.Where(b => EF.Functions.ILike(b.Author, $"%{author}%"));
        }

        // 3. فلترة محددة بالتصنيف
        if (filter.CategoryId.HasValue && filter.CategoryId.Value != Guid.Empty)
        {
            query = query.Where(b => b.CategoryId == filter.CategoryId.Value);
        }

        // 4. حساب العدد الإجمالي
        var totalCount = await query.CountAsync(ct);

        // 5. ضبط قيم الترقيم بأمان
        var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
        var pageSize = filter.PageSize < 1 ? 10 : (filter.PageSize > 100 ? 100 : filter.PageSize);

        // 6. جلب الصفحة المطلوبة مع الإسقاط المباشر
        var items = await query
            .OrderBy(b => b.Title)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookDto(b.Id, b.Title, b.Author, b.Isbn, b.CategoryId, b.Category.Name))
            .ToListAsync(ct);

        return new PagedResult<BookDto>(items, pageNumber, pageSize, totalCount);
    }
    
    public async Task<BookDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Books
            .AsNoTracking()
            .Where(b => b.Id == id)
            .Select(b => new BookDto(b.Id, b.Title, b.Author, b.Isbn, b.CategoryId, b.Category.Name))
            .FirstOrDefaultAsync(ct);
    }
}
