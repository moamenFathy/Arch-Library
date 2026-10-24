using Library.Modules.Catalog.Application.Categories;
using Library.Modules.Catalog.Domain;

namespace Library.Modules.Catalog.Application;

public interface ICategoryRepository
{
  Task AddCategoryAsync(Category category, CancellationToken ct = default);
  Task<Category?> GetCategoryByIdAsync(Guid id, CancellationToken ct = default);
  Task<bool> CategoryExistsAsync(string name, CancellationToken ct = default);
  Task SaveChangesAsync(CancellationToken ct = default);
}