namespace Library.Modules.Catalog.Application.Books;

public record CreateBookCommand(string Title, string Author, string Isbn);