using Library.Modules.Catalog.Domain;
using Library.Shared.Exceptions;

namespace Library.Modules.Catalog.Application.Books;

public class BookService(IBookRepository bookRepository)
{
    public async Task<Guid> CreateBookAsync(CreateBookCommand command, CancellationToken ct = default)
    {
        if (await bookRepository.IsbnExistsAsync(command.Isbn, ct))
        {
            throw AppException.Conflict($"A book with ISBN '{command.Isbn}' already exists.");
        }

        var book = Book.Create(command.Title, command.Author, command.Isbn);
        await bookRepository.AddAsync(book, ct);
        await bookRepository.SaveChangesAsync(ct);

        return book.Id;
    }

    public async Task UpdateBookAsync(UpdateBookCommand command, CancellationToken ct = default)
    {
        var book = await bookRepository.GetByIdAsync(command.Id, ct);
        if (book is null)
        {
            throw AppException.NotFound($"Book with ID '{command.Id}' was not found.");
        }

        book.Update(command.Title, command.Author);
        await bookRepository.SaveChangesAsync(ct);
    }
}
