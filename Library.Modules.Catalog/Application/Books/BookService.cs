using Library.Modules.Catalog.Domain;

namespace Library.Modules.Catalog.Application.Books;

public class BookService
{
    private readonly IBookRepository _bookRepository;
    
    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }
    
    public async Task<Guid> CreateBookAsync(CreateBookCommand command, CancellationToken ct = default)
    {
        if (await _bookRepository.IsbnExistsAsync(command.Isbn, ct))
        {
            throw new InvalidOperationException($"A book with ISBN '{command.Isbn}' already exists.");
        }

        var book = Book.Create(command.Title, command.Author, command.Isbn);
        await _bookRepository.AddAsync(book, ct);
        await _bookRepository.SaveChangesAsync(ct);

        return book.Id;
    }
    
    public async Task<bool> UpdateBookAsync(UpdateBookCommand command, CancellationToken ct = default)
    {
        var book = await _bookRepository.GetByIdAsync(command.Id, ct);
        if (book is null)
        {
            throw new KeyNotFoundException($"A book with ID '{command.Id}' does not exist.");
        }

        book.Update(command.Title, command.Author);
        await _bookRepository.SaveChangesAsync(ct);
        return true;
    }
}