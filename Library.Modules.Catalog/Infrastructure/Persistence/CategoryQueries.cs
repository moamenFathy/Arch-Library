using Library.Modules.Catalog.Application.Categories;
using Microsoft.EntityFrameworkCore;

namespace Library.Modules.Catalog.Infrastructure.Persistence;

public class CategoryQueries(CatalogDbContext context) : ICategoryQueries
{
    // get all with no tracking
    public async Task<List<CategoryDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Categories
            .AsNoTracking()
            .Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.IsActive))
            .ToListAsync(ct);
    }

    // get by id with no tracking
    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Categories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.IsActive))
            .FirstOrDefaultAsync(ct);
    }
}
