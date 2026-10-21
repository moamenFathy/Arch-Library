using Library.Modules.Catalog;
using Library.Modules.Catalog.Application.Books;
using Library.Modules.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCatalogModule(builder.Configuration);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Endpoint لقراءة كل الكتب
app.MapGet("/api/books", async (BookQueries queries, CancellationToken ct) =>
{
    var books = await queries.GetAllAsync(ct);
    return Results.Ok(books);
});

// Endpoint لقراءة كتاب محدد
app.MapGet("/api/books/{id:guid}", async (Guid id, BookQueries queries, CancellationToken ct) =>
{
    var book = await queries.GetByIdAsync(id, ct);
    return book is not null ? Results.Ok(book) : Results.NotFound();
});

// Endpoint لإنشاء كتاب جديد
app.MapPost("/api/books", async (CreateBookCommand command, BookService service, CancellationToken ct) =>
{
    var bookId = await service.CreateBookAsync(command, ct);
    return Results.Created($"/api/books/{bookId}", new { Id = bookId });
});

// Endpoint لتحديث كتاب موجود
app.MapPut("/api/books/{id:guid}", async (Guid id, UpdateBookRequest request, BookService service, CancellationToken ct) =>
{
    var command = new UpdateBookCommand(id, request.Title, request.Author);
    var updated = await service.UpdateBookAsync(command, ct);

    return updated ? Results.NoContent() : Results.NotFound();
});

app.Run();