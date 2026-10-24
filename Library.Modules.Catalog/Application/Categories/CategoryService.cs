using Library.Modules.Catalog.Domain;
using Library.Shared.Exceptions;

namespace Library.Modules.Catalog.Application.Categories;

public class CategoryService(ICategoryRepository categoryRepository)
{
    public async Task<Guid> CreateCategoryAsync(CreateCategoryCommand command, CancellationToken ct = default)
    {
        if (await categoryRepository.CategoryExistsAsync(command.Name, ct))
        {
            throw AppException.Conflict($"Category with name '{command.Name}' already exists.");
        }

        var category = Category.Create(command.Name, command.Description, command.IsActive);
        await categoryRepository.AddCategoryAsync(category, ct);
        await categoryRepository.SaveChangesAsync(ct);

        return category.Id;
    }

    public async Task UpdateCategoryAsync(UpdateCategoryCommand command, CancellationToken ct = default)
    {
        var category = await categoryRepository.GetCategoryByIdAsync(command.Id, ct);
        if (category is null)
        {
            throw AppException.NotFound($"Category with ID '{command.Id}' was not found.");
        }

        category.Update(command.Name, command.Description, command.IsActive);
        await categoryRepository.SaveChangesAsync(ct);
    }

    public async Task<bool> CategoryExistsAsync(string name, CancellationToken ct = default)
    {
        return await categoryRepository.CategoryExistsAsync(name, ct);
    }
}
