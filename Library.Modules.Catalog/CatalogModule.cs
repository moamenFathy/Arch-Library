// CatalogModule.cs
using Library.Modules.Catalog.Application;
using Library.Modules.Catalog.Application.Books;
using Library.Modules.Catalog.Application.Categories;
using Library.Modules.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Modules.Catalog;

public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "catalog")));

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<BookService>();
        services.AddScoped<IBookQueries, BookQueries>();
        
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<CategoryService>();
        services.AddScoped<ICategoryQueries, CategoryQueries>();

        return services;
    }
}
