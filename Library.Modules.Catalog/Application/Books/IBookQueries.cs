using Library.Shared.Common;

namespace Library.Modules.Catalog.Application.Books;

public interface IBookQueries
{
    Task<PagedResult<BookDto>> GetAllAsync(BookFilterParams filter, CancellationToken ct = default);
    Task<BookDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
