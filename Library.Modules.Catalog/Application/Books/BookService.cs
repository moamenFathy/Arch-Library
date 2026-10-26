using Library.Modules.Catalog.Domain;
using Library.Shared.Exceptions;

namespace Library.Modules.Catalog.Application.Books;

public class BookService(IBookRepository bookRepository, ICategoryRepository categoryRepository)
{
    public async Task<Guid> CreateBookAsync(CreateBookCommand command, CancellationToken ct = default)
    {
        // التحقق من وجود التصنيف أولاً
        var category = await categoryRepository.GetCategoryByIdAsync(command.CategoryId, ct);
        if (category is null)
        {
            throw AppException.NotFound($"Category with ID '{command.CategoryId}' was not found.");
        }

        if (await bookRepository.IsbnExistsAsync(command.Isbn, ct))
        {
            throw AppException.Conflict($"A book with ISBN '{command.Isbn}' already exists.");
        }

        var book = Book.Create(command.Title, command.Author, command.Isbn, command.CategoryId);
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

        // التحقق من وجود التصنيف في حالة تعديله
        var category = await categoryRepository.GetCategoryByIdAsync(command.CategoryId, ct);
        if (category is null)
        {
            throw AppException.NotFound($"Category with ID '{command.CategoryId}' was not found.");
        }

        book.Update(command.Title, command.Author, command.CategoryId);
        await bookRepository.SaveChangesAsync(ct);
    }
}
