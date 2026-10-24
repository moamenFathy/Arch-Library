using Library.Modules.Catalog.Application.Categories;
using Microsoft.EntityFrameworkCore;

namespace Library.Modules.Catalog.Infrastructure.Persistence;

public class CategoryQueries(CatalogDbContext context)
{
  private readonly CatalogDbContext _context = context;
  
  // get all
  public async Task<List<CategoryDto>> GetAllAsync(CancellationToken ct = default)
  {
    return await _context.Categories
      .Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.IsActive))
      .ToListAsync(ct);
  }

  // get by id
  public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id, CancellationToken ct = default)
  {
    return await _context.Categories
      .Where(c => c.Id == id)
      .Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.IsActive))
      .FirstOrDefaultAsync(ct);
  }
}