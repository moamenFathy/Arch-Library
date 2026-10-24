namespace Library_Api.Common.Contracts;

public record UpdateCategoryRequest(string Name, string? Description, bool IsActive);