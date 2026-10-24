namespace Library.Modules.Catalog.Application.Categories;

public record CreateCategoryCommand(string Name, string? Description, bool IsActive);