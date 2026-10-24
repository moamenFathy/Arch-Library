using Library.Modules.Catalog.Domain;

namespace Library.Modules.Catalog.Application.Categories;

public class CategoryService(ICategoryRepository categoryRepository)
{
  private readonly ICategoryRepository _categoryRepository = categoryRepository;
  
  // create a new category
  public async Task<Guid> CreateCategoryAsync(CreateCategoryCommand command, CancellationToken ct = default)
  {
    
    if (await _categoryRepository.CategoryExistsAsync(command.Name, ct))
    {
      throw new InvalidOperationException($"Category with name '{command.Name}' already exists.");
    }
    
    var category = Category.Create(command.Name, command.Description, command.IsActive);

    await _categoryRepository.AddCategoryAsync(category, ct);
    await _categoryRepository.SaveChangesAsync(ct);
    
    return category.Id;
  }
  
  // update a category
  public async Task<bool> UpdateCategoryAsync(Guid id, UpdateCategoryCommand command, CancellationToken ct = default)
  {
    var category = await _categoryRepository.GetCategoryByIdAsync(id, ct);
    if (category is null)
    {
      return false;
    }

    category.Update(command.Name, command.Description, command.IsActive);

    await _categoryRepository.SaveChangesAsync(ct);

    return true;
  }

  // is category exists
  public async Task<bool> CategoryExistsAsync(string name, CancellationToken ct = default)
  {
    return await _categoryRepository.CategoryExistsAsync(name, ct);
  }
}