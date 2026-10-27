namespace Library.Modules.Catalog.Application.Books;

public record BookFilterParams(
    string? SearchTerm = null,
    string? Author = null,
    Guid? CategoryId = null,
    int PageNumber = 1,
    int PageSize = 10);
