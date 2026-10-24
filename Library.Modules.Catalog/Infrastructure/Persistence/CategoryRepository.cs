using Library.Modules.Catalog.Application;
using Library.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;

namespace Library.Modules.Catalog.Infrastructure.Persistence;

public class CategoryRepository(CatalogDbContext context) : ICategoryRepository
{
  private readonly CatalogDbContext _context = context;

  public async Task AddCategoryAsync(Category category, CancellationToken ct = default) =>
    await _context.AddAsync(category, ct);

  public async Task<Category?> GetCategoryByIdAsync(Guid id, CancellationToken ct = default) =>
    await _context.Categories.FindAsync([id], ct);

  public async Task<bool> CategoryExistsAsync(string name, CancellationToken ct = default) =>
    await _context.Categories.AnyAsync(c => c.Name == name, ct);

  public async Task SaveChangesAsync(CancellationToken ct = default) => await _context.SaveChangesAsync(ct);
}