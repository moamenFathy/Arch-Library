// Domain/Books/Book.cs
namespace Library.Modules.Catalog.Domain;

public class Book
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = null!;
    public string Author { get; private set; } = null!;
    public string Isbn { get; private set; } = null!;

    private Book() { }

    public static Book Create(string title, string author, string isbn)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required");
        if (string.IsNullOrWhiteSpace(author)) throw new ArgumentException("Author is required");
        if (string.IsNullOrWhiteSpace(isbn)) throw new ArgumentException("Isbn is required");

        return new Book { Id = Guid.NewGuid(), Title = title, Author = author, Isbn = isbn };
    }
}