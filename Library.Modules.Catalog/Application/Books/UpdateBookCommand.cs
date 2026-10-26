namespace Library.Modules.Catalog.Application.Books;

public record UpdateBookCommand(Guid Id, string Title, string Author, Guid CategoryId);
