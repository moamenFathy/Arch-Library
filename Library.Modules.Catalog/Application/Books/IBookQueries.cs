namespace Library.Modules.Catalog.Application.Books;

public interface IBookQueries
{
    Task<List<BookDto>> GetAllAsync(CancellationToken ct = default);
    Task<BookDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
