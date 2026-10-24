using Library_Api.Common;
using Library_Api.Common.Contracts;
using Library.Modules.Catalog.Application.Categories;
using Library.Modules.Catalog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace Library_Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(CategoryQueries queries, CategoryService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var categories = await queries.GetAllAsync(ct);
        return Ok(ApiResponse<IReadOnlyList<CategoryDto>>.Success(categories, "Categories retrieved successfully"));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var category = await queries.GetCategoryByIdAsync(id, ct);
        if (category is null)
        {
            return NotFound(ApiResponse<CategoryDto>.Failure($"Category with ID '{id}' was not found."));
        }

        return Ok(ApiResponse<CategoryDto>.Success(category, "Category retrieved successfully"));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command, CancellationToken ct)
    {
        var categoryId = await service.CreateCategoryAsync(command, ct);
        var response = ApiResponse<Guid>.Success(categoryId, "Category created successfully");

        return CreatedAtAction(nameof(GetById), new { id = categoryId }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken ct)
    {
        var command = new UpdateCategoryCommand(id, request.Name, request.Description, request.IsActive);
        await service.UpdateCategoryAsync(command, ct);

        return Ok(ApiResponse.Success("Category updated successfully"));
    }
}
