namespace Library.Modules.Catalog.Application.Categories;

public record CategoryDto(Guid Id, string Name, string? Description, bool IsActive);