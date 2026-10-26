namespace Library.Modules.Catalog.Application.Categories;

public interface ICategoryQueries
{
    Task<List<CategoryDto>> GetAllAsync(CancellationToken ct = default);
    Task<CategoryDto?> GetCategoryByIdAsync(Guid id, CancellationToken ct = default);
}
