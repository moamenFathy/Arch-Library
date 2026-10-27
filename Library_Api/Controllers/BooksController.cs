using Library_Api.Common;
using Library_Api.Common.Contracts;
using Library.Modules.Catalog.Application.Books;
using Library.Shared.Common;
using Microsoft.AspNetCore.Mvc;

namespace Library_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController(IBookQueries queries, BookService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] BookFilterParams filter, CancellationToken ct)
    {
        var books = await queries.GetAllAsync(filter, ct);
        return Ok(ApiResponse<PagedResult<BookDto>>.Success(books, "Books retrieved successfully"));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var book = await queries.GetByIdAsync(id, ct);
        if (book is null)
        {
            return NotFound(ApiResponse<BookDto>.Failure($"Book with ID '{id}' was not found."));
        }

        return Ok(ApiResponse<BookDto>.Success(book, "Book retrieved successfully"));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookCommand command, CancellationToken ct)
    {
        var bookId = await service.CreateBookAsync(command, ct);
        var response = ApiResponse<Guid>.Success(bookId, "Book created successfully");
        
        return CreatedAtAction(nameof(GetById), new { id = bookId }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookRequest request, CancellationToken ct)
    {
        var command = new UpdateBookCommand(id, request.Title, request.Author, request.CategoryId);
        await service.UpdateBookAsync(command, ct);

        return Ok(ApiResponse.Success("Book updated successfully"));
    }
}
