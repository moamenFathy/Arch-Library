# AGENTS.md — Cortexa Architecture & Implementation Guidelines for AI Agents

> **Purpose & Role:**  
> This document is the definitive specification and operational playbook for AI coding agents working within this repository. All code generated, refactored, or reviewed by AI agents must strictly comply with the architectural decisions, structural patterns, and coding standards defined here.

---

## 1. System Overview & Core Philosophy

Cortexa is designed as a structured, interconnected, and AI-accessible system built around clear ownership, explicit dependencies, reliable data, and useful abstractions.

### Core Architectural Principles

1. **Modularity (Modular Monolith):** A single deployable application organized into cohesive, bounded modules. A module represents a business capability, not just a technical layer or folder.
2. **Hexagonal Architecture (Ports & Adapters):** Strict separation between core business rules and external dependencies (databases, web frameworks, external AI providers, etc.). Dependencies always point inward.
3. **Data Integrity & Authoritative Source of Truth:** The primary database is the sole authoritative source of truth. Search indexes, vector embeddings, and caches are derived data and must never compete as sources of truth.
4. **Pragmatic CQRS-Lite:** Separation of write operations (orchestrated via Domain entities and repositories) from read operations (optimized queries projecting directly to DTOs).
5. **Pragmatism over Dogmatism:** Introduce architectural patterns only when they solve a proven problem. Speculative abstractions (e.g., generic repositories, redundant Unit of Work wrappers, unnecessary MediatR pipelines) are strictly forbidden.

---

## 2. Architectural Layers & Inward Dependency Rule

Dependencies must flow inward: **Delivery → Infrastructure → Application → Domain**.

```
┌────────────────────────────────────────────────────────┐
│                   Delivery Layer (API)                 │
│      Controllers, Endpoints, DTO Requests/Responses    │
└──────────────────────────┬─────────────────────────────┘
                           │ uses
┌──────────────────────────▼─────────────────────────────┐
│                   Infrastructure                       │
│    EF Core DbContext, Repositories, Queries, Mappings, │
│             External Services, AI Clients              │
└──────────────────────────┬─────────────────────────────┘
                           │ implements / depends on
┌──────────────────────────▼─────────────────────────────┐
│                    Application                         │
│    Use Cases, Commands, Services, Ports/Interfaces,    │
│                     Query Models                       │
└──────────────────────────┬─────────────────────────────┘
                           │ depends on
┌──────────────────────────▼─────────────────────────────┐
│                      Domain                            │
│     Entities, Value Objects, Business Invariants,      │
│                Domain Events, Rules                    │
│           (ZERO external framework dependencies)       │
└────────────────────────────────────────────────────────┘
```

### Layer Responsibilities & Rules

| Layer | Responsibilities | Constraints |
|---|---|---|
| **Domain** | Entities, Value Objects, Business Invariants, Factory Methods, Domain Events. | **Zero dependencies.** No ASP.NET Core, EF Core, database drivers, or vendor AI SDKs. |
| **Application** | Use cases, Services, Command handlers, Repository interfaces (ports), Application DTOs, Validation. | Depends only on Domain. Does not depend on concrete persistence or HTTP delivery frameworks. |
| **Infrastructure** | EF Core `DbContext`, Migrations, Repository implementations, Query implementations, External AI adapters, File storage. | Implements Application interfaces. Houses DB schemas, queries (`AsNoTracking`), and SDK integrations. |
| **Delivery (API)** | HTTP Controllers, API contracts, Route mapping, Middleware, Global Exception Handling, DI Module extensions. | Translates external HTTP requests into Application calls. Does **not** contain business rules. |

---

## 3. Standard Module Structure

Every module in the solution must adhere to the following directory layout:

```text
Library.Modules.<ModuleName>/
├── Domain/
│   ├── <Entity>.cs                  # Encapsulated entity with factory & business methods
│   ├── ValueObjects/                # Immutable value objects
│   └── Events/                      # Domain events (if justified)
├── Application/
│   ├── <Feature>/
│   │   ├── <Entity>Dto.cs           # Read/Result data transfer objects (records)
│   │   ├── <Action><Entity>Command.cs # Command records for mutations
│   │   └── <Entity>Service.cs       # Coordinates mutations and business workflows
│   ├── I<Entity>Repository.cs       # Persistence contracts (Ports)
│   └── Common/                      # Feature-agnostic application contracts
├── Infrastructure/
│   ├── Persistence/
│   │   ├── <ModuleName>DbContext.cs # EF Core DbContext isolated to module schema
│   │   ├── Configuration/           # IEntityTypeConfiguration<T> Fluent API mappings
│   │   ├── <Entity>Repository.cs    # Write-path persistence implementation
│   │   └── <Entity>Queries.cs       # Read-path optimized queries (AsNoTracking)
│   └── Migrations/                  # EF Core migrations scoped to module schema
└── <ModuleName>Module.cs            # IServiceCollection DI registration extension
```

---

## 4. Module Boundaries & Communication Rules

AI agents must enforce module isolation at all times:

1. **Autonomous Persistence:** Each module manages its own tables and migrations inside a dedicated PostgreSQL schema (e.g., `options.UseNpgsql(connStr, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "catalog"))`).
2. **No Direct Database Sharing:** A module must **never** query or reference another module's `DbContext`, tables, or internal entities.
3. **Public Application Contracts:** Inter-module communication must take place only through:
   - Public Application interfaces/contracts exposed by the owning module.
   - Domain / Integration Events.
   - Public Application DTOs.
4. **No Circular Dependencies:** Module dependencies must be unidirectional. If Module A depends on Module B, Module B must never depend on Module A.
5. **Self-Contained Dependency Injection:** Each module must provide an extension method `Add<ModuleName>Module(this IServiceCollection services, IConfiguration config)` that registers its own DbContext, Repositories, Services, and Queries.

---

## 5. Persistence & CQRS-Lite Implementation

### 5.1 Domain Entity Encapsulation
- **No Public Setters:** Entities must use private or init-only setters.
- **Factory Methods:** Entity instances must be instantiated via explicit factory methods (e.g., `Book.Create(...)`) that validate domain invariants before instantiation.
- **Business Mutation Methods:** State changes must happen through semantic methods (e.g., `book.Update(...)`, `page.MoveTo(...)`), never by property assignment.
- **Identifiers:** Use `Guid` (or strongly-typed IDs) generated at creation time.

```csharp
// Example: Domain/Book.cs
public class Book
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = null!;
    public string Author { get; private set; } = null!;
    public string Isbn { get; private set; } = null!;

    private Book() { } // Required by EF Core

    public static Book Create(string title, string author, string isbn)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required");
        if (string.IsNullOrWhiteSpace(author)) throw new ArgumentException("Author is required");
        if (string.IsNullOrWhiteSpace(isbn)) throw new ArgumentException("Isbn is required");

        return new Book { Id = Guid.NewGuid(), Title = title, Author = author, Isbn = isbn };
    }

    public void Update(string title, string author)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required");
        if (string.IsNullOrWhiteSpace(author)) throw new ArgumentException("Author is required");

        Title = title;
        Author = author;
    }
}
```

### 5.2 The Write Path (Commands & Repositories)
- Repositories handle write-side persistence operations: `GetByIdAsync`, `AddAsync`, `SaveChangesAsync`.
- Do **not** create a generic `IRepository<T>`. Define focused, explicit repository interfaces per aggregate (e.g., `IBookRepository`).
- EF Core's `DbContext` is already a Unit of Work. Do **not** build a custom `IUnitOfWork` wrapper unless explicitly required by multi-module transactions.

```csharp
// Example: Infrastructure/Persistence/BookRepository.cs
public class BookRepository(CatalogDbContext context) : IBookRepository
{
    public async Task AddAsync(Book book, CancellationToken ct = default)
        => await context.Books.AddAsync(book, ct);

    public Task<Book?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => context.Books.FirstOrDefaultAsync(b => b.Id == id, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => context.SaveChangesAsync(ct);
}
```

### 5.3 The Read Path (Queries)
- Read operations bypass repositories and use dedicated Query classes (e.g., `BookQueries`).
- Use `.AsNoTracking()` and project directly to DTOs via `.Select(...)`.
- This avoids unnecessary change-tracking overhead and keeps read operations lightweight and performant.

```csharp
// Example: Infrastructure/Persistence/BookQueries.cs
public class BookQueries(CatalogDbContext context)
{
    public async Task<List<BookDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Books
            .AsNoTracking()
            .Select(b => new BookDto(b.Id, b.Title, b.Author, b.Isbn))
            .ToListAsync(ct);
    }

    public async Task<BookDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Books
            .AsNoTracking()
            .Where(b => b.Id == id)
            .Select(b => new BookDto(b.Id, b.Title, b.Author, b.Isbn))
            .FirstOrDefaultAsync(ct);
    }
}
```

---

## 6. Delivery Layer & API Standards

1. **Standardized Response Envelope:** All HTTP endpoints should return responses wrapped in `ApiResponse<T>`:
   ```csharp
   ApiResponse<T>.SuccessResponse(data, message, statusCode);
   ApiResponse<T>.FailureResponse(errorMessage, statusCode, errors);
   ```
2. **Centralized Error Handling:**
   - Use `GlobalExceptionHandler` (`IExceptionHandler`) with `ProblemDetails`.
   - Map domain/application exceptions (e.g., `KeyNotFoundException` → 404, `ArgumentException` or `InvalidOperationException` → 400/409) without leaking stack traces.
3. **Thin Controllers:**
   - Controllers only validate HTTP request bounds, delegate to `Queries` (for reads) or `Services` (for writes), and return standard `ApiResponse<T>`.
   - Never place business logic, SQL queries, or direct entity modifications inside controllers.

---

## 7. AI & Search Integration Guidelines

When implementing Cortexa knowledge workspace capabilities (Pages, Blocks, Search, AI Integration):

1. **Search vs. AI Retrieval Separation:** Traditional search (lexical matching, filtering, ranking) and AI retrieval (embeddings, semantic search, context assembly) are separate concerns. Keep them decoupled.
2. **Provider Isolation (Ports & Adapters):** Never couple domain or application logic to specific vendor SDKs (e.g., OpenAI, Google Gemini, Anthropic). Define abstractions in Application and implement adapters in Infrastructure.
3. **Authorization Parity:** AI retrieval and search must enforce the exact same access control and authorization policies as direct resource access.
4. **AI Output Is Non-Authoritative:** AI-generated content must pass standard domain validations before persistence. AI does not bypass business rules.
5. **Context Assembly & Token Budgets:** Retrieval pipelines must structure context with explicit source references (page/block IDs) and respect token boundaries.

---

## 8. Coding Standards & Conventions

AI agents must write code consistent with modern C# (.NET 10 / C# 13) idioms:

- **File-Scoped Namespaces:** Always use `namespace Library.Modules.<Name>;`.
- **Primary Constructors:** Use primary constructors for dependency injection in classes (e.g., `public class BookQueries(CatalogDbContext context)`).
- **Records for DTOs & Commands:** Use immutable records for DTOs and commands:
  ```csharp
  public record BookDto(Guid Id, string Title, string Author, string Isbn);
  public record CreateBookCommand(string Title, string Author, string Isbn);
  ```
- **Async & Cancellation:** All I/O operations must be asynchronous and accept `CancellationToken ct = default`.
- **Nullable Reference Types:** Nullable annotations are enabled. Use `null!` only for EF Core backing properties initialized by reflection.
- **EF Core Configuration:** Use `IEntityTypeConfiguration<T>` in separate mapping files inside `Infrastructure/Persistence/Configuration/` rather than cluttering `OnModelCreating`.

---

## 9. Agent Commandments (Rules of Engagement)

### MUST DO (Agent Checklist)
- [ ] **Check Module Boundaries:** Keep every new feature strictly within its designated module.
- [ ] **Preserve Domain Purity:** Ensure Domain projects have zero NuGet references to web or persistence libraries.
- [ ] **Enforce Entity Encapsulation:** Guard invariants using private setters and `Create(...)`/`Update(...)` methods.
- [ ] **Use CQRS-Lite:** Direct query classes with `.AsNoTracking()` for reads; Services + Repositories for writes.
- [ ] **Scope DB Schemas:** Always configure PostgreSQL migrations to use module-specific schemas (`catalog`, `lending`, `members`, `cortexa_pages`, etc.).
- [ ] **Register Dependencies in Module Extensions:** Add all services, repos, and queries in `<ModuleName>Module.cs`.
- [ ] **Return Standard Envelopes:** Wrap controller responses in `ApiResponse<T>`.

### MUST NOT DO (Strictly Forbidden)
- ❌ **Do NOT reference another module's DbContext or internal entities directly.**
- ❌ **Do NOT create generic repositories (`IRepository<T>`) or custom Unit of Work wrappers.**
- ❌ **Do NOT introduce MediatR or complex messaging buses unless explicitly approved in an ADR.**
- ❌ **Do NOT expose Domain Entities directly in API Controller responses (always project to DTOs).**
- ❌ **Do NOT embed external AI SDKs directly into Domain or Application layers.**
- ❌ **Do NOT write business validation logic inside API Controllers.**

---

## 10. Architecture Decision Records (ADR)

When proposing a structural change, new library, or architectural shift:
1. Document the context, decision, alternatives, and consequences in `docs/architecture/decisions/`.
2. Follow naming convention: `ADR-XXX-<decision-title>.md`.
3. Consult existing ADRs before introducing architectural variations.
