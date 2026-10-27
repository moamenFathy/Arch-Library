using Library.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Library.Modules.Catalog.Infrastructure.Persistence;

public static class CatalogDataSeeder
{
    public static async Task SeedAsync(CatalogDbContext context, ILogger? logger = null)
    {
        // 1. فحص هل قاعدة البيانات تحتوي بالفعل على بيانات لتجنب التكرار
        if (await context.Books.AnyAsync())
        {
            logger?.LogInformation("Catalog database already contains books. Skipping seeding.");
            return;
        }

        logger?.LogInformation("Starting Catalog database seeding (10 Categories + 1,000 Books)...");

        // 2. إنشاء 10 تصنيفات تقنية احترافية
        var categoryDefs = new (string Name, string Description)[]
        {
            ("Software Architecture", "Design patterns, clean code, modular monoliths, and enterprise systems."),
            ("Domain-Driven Design", "Tactical and strategic DDD, bounded contexts, and aggregate patterns."),
            ("Cloud & Distributed Systems", "Microservices, distributed messaging, consensus, and cloud platforms."),
            ("Databases & Data Engineering", "Relational databases, PostgreSQL, NoSQL, indexing, and optimization."),
            ("Artificial Intelligence", "Machine learning, neural networks, LLMs, and intelligent systems."),
            ("DevOps & Site Reliability", "Docker, Kubernetes, CI/CD pipelines, and observability engineering."),
            ("Cybersecurity & Cryptography", "Application security, ethical hacking, identity, and zero trust."),
            ("Web & API Development", "REST APIs, GraphQL, ASP.NET Core, and modern web architectures."),
            ("Algorithms & Data Structures", "Computational complexity, sorting, graphs, and algorithmic problem solving."),
            ("Programming Languages", "Compilers, type systems, functional programming, and language design.")
        };

        var categories = new List<Category>();
        foreach (var def in categoryDefs)
        {
            // استخدام Factory Method لضمان صحة الـ Domain
            var category = Category.Create(def.Name, def.Description, true);
            categories.Add(category);
        }

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        logger?.LogInformation("Seeded {CategoryCount} categories successfully.", categories.Count);

        // 3. مكونات توليد عناوين وأسماء كتب ومؤلفين واقعية
        var prefixes = new[]
        {
            "Mastering", "Fundamentals of", "Practical", "Building", "Designing",
            "Advanced", "Patterns in", "The Art of", "Modern", "Essential",
            "Deep Dive into", "Engineering", "Production-Ready", "Hands-On", "The Definitive Guide to"
        };

        var topics = new[]
        {
            "Clean Architecture", "PostgreSQL Internals", "Distributed Transactions", "Kubernetes at Scale",
            "Domain Modeling", "ASP.NET Core Systems", "Event-Driven Systems", "OAuth2 & OpenID Connect",
            "High-Performance C#", "Graph Databases", "Microservices Resiliency", "Concurrent Programming",
            "Observability & Tracing", "API Gateway Design", "Database Indexing & Sharding", "Network Protocols",
            "CQRS & Event Sourcing", "Container Security", "Compiler Construction", "Vector Databases & AI Search"
        };

        var suffixes = new[]
        {
            "in Practice", "for Professionals", ": A Deep Dive", "Handbook",
            "from Zero to Hero", "with C# & .NET", "for Enterprise", "Cookbook",
            "Principles & Practices", "Architecture & Implementation"
        };

        var authors = new[]
        {
            "Martin Fowler", "Robert C. Martin", "Eric Evans", "Kent Beck",
            "Greg Young", "Vaughn Vernon", "Brendan Burns", "Martin Kleppmann",
            "Sam Newman", "Michael Nygard", "Scott Wlaschin", "Andrew Lock",
            "Jon Skeet", "Mark Seemann", "Nick Chapsas", "Milan Jovanovic"
        };

        // 4. توليد 1,000 كتاب برمجياً
        var books = new List<Book>(1000);
        var totalCategories = categories.Count;

        for (int i = 1; i <= 1000; i++)
        {
            var prefix = prefixes[(i * 7) % prefixes.Length];
            var topic = topics[(i * 13) % topics.Length];
            var suffix = suffixes[(i * 11) % suffixes.Length];
            var author = authors[(i * 17) % authors.Length];
            var category = categories[i % totalCategories];

            var title = $"{prefix} {topic} {suffix}";

            // توليد رقم ISBN فريد وصحيح مكون من 13 رقماً
            var isbn = $"978-{i % 9:0}-{1000 + (i * 3):0000}-{i:0000}-{i % 10}";

            var book = Book.Create(title, author, isbn, category.Id);
            books.Add(book);
        }

        // 5. حفظ الـ 1,000 كتاب دفعة واحدة فائقة السرعة (Batch Insert)
        await context.Books.AddRangeAsync(books);
        await context.SaveChangesAsync();

        logger?.LogInformation("Successfully seeded 1,000 books across {CategoryCount} categories!", totalCategories);
    }
}
