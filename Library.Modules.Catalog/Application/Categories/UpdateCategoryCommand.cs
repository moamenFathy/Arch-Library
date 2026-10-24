namespace Library.Modules.Catalog.Application.Categories;

public record UpdateCategoryCommand(Guid Id, string Name, string? Description, bool IsActive);