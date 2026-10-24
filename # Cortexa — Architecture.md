# Cortexa — Architecture

## 1. Overview

Cortexa is an AI-accessible knowledge workspace designed to organize information into structured, interconnected, and searchable content.

The system aims to provide a structured knowledge base that both humans and AI systems can navigate, understand, and use as context.

The architecture should support an initial, manageable implementation while allowing the system to evolve as its requirements become clearer.

### Core Architectural Goals

- **Modularity:** Organize the system into cohesive modules with clear responsibilities.
- **Maintainability:** Keep business rules independent of infrastructure and delivery mechanisms.
- **Extensibility:** Allow new capabilities to be introduced without unnecessarily affecting existing modules.
- **Data Integrity:** Enforce important relationships and business invariants consistently.
- **Testability:** Make business logic testable without requiring a running database or external services.
- **AI Readiness:** Provide structured, searchable, and retrievable knowledge that AI features can consume.
- **Pragmatism:** Introduce architectural complexity only when it solves a demonstrated problem.

---

## 2. Architectural Approach

The initial architectural direction is a **Modular Monolith combined with Hexagonal Architecture**.

These concepts address different architectural concerns and complement each other.

### 2.1 Modular Monolith

Cortexa will initially be designed as a single deployable application organized into distinct modules.

Each module should own a well-defined responsibility and expose only the contracts required by other modules.

The modular monolith approach aims to provide:

- Clear boundaries between business capabilities.
- Straightforward local development and deployment.
- Lower operational complexity than a distributed system.
- The ability to evolve modules independently within the application.
- A potential path toward distributed deployment if future requirements justify it.

A module boundary should represent a meaningful responsibility, not merely a folder or technical layer.

### 2.2 Hexagonal Architecture

Hexagonal Architecture, also known as Ports and Adapters, guides the separation between application logic and external dependencies.

The central principle is that business rules should not depend directly on implementation details such as databases, web frameworks, or external AI providers.

The architecture distinguishes between:

- **Domain:** Business concepts, rules, and invariants.
- **Application:** Use cases, orchestration, and the contracts required to execute them.
- **Infrastructure:** Concrete implementations for persistence and external integrations.
- **Delivery Adapters:** Mechanisms through which users or external systems interact with the application, such as HTTP APIs.

Dependencies should point toward the application's core rather than allowing infrastructure concerns to dictate business logic.

---

## 3. Architectural Layers

### 3.1 Domain

The Domain layer contains the business model and rules that define the system's behavior.

Responsibilities may include:

- Domain entities and value objects.
- Business invariants.
- Domain services when a rule does not naturally belong to one entity.
- Domain events when they provide a clear benefit.

The Domain layer should not depend on ASP.NET Core, Entity Framework Core, database providers, or external AI SDKs.

### 3.2 Application

The Application layer coordinates operations that fulfill user and system requirements.

Responsibilities may include:

- Use cases and application services.
- Input validation related to application operations.
- Authorization requirements at the use-case boundary where appropriate.
- Interfaces for required external capabilities.
- Transaction and persistence coordination where required.
- Application-level query and result models.

The Application layer should depend on Domain concepts and abstractions rather than concrete infrastructure implementations.

### 3.3 Infrastructure

The Infrastructure layer implements technical capabilities required by the application.

Responsibilities may include:

- Database access and persistence.
- Entity Framework Core configurations and mappings.
- Repository implementations where repositories are justified.
- External service integrations.
- AI provider integrations.
- Search engine integrations.
- File and object storage integrations.

Infrastructure implementations should be replaceable without rewriting core business rules, provided the relevant contracts remain stable.

### 3.4 Delivery Layer

The delivery layer exposes Cortexa capabilities to users and external consumers.

It may include:

- HTTP API endpoints.
- Request and response models.
- Authentication integration.
- Request-level validation and error handling.
- Dependency injection and application startup configuration.

Delivery code should translate external requests into application operations rather than implementing business rules directly.

---

## 4. Proposed Module Boundaries

The following modules represent an initial conceptual decomposition. Their final boundaries and implementation details remain subject to validation as requirements evolve.

### 4.1 Pages

Responsible for the lifecycle and organization of knowledge pages.

Potential responsibilities:

- Creating, updating, and retrieving pages.
- Managing page metadata.
- Organizing hierarchical page relationships.
- Enforcing page-level business rules.

### 4.2 Blocks

Responsible for the structured content contained within pages.

Potential responsibilities:

- Representing different content block types.
- Managing block ordering and positioning.
- Updating and validating block content.
- Supporting the addition of new block types.

The content model should distinguish between stable structural metadata and type-specific content.

### 4.3 Relations

Responsible for explicit relationships between knowledge objects where these relationships require their own rules or lifecycle.

Potential responsibilities:

- Defining relationships between supported objects.
- Validating relationship constraints.
- Supporting navigation across connected knowledge.
- Maintaining relationship integrity.

Simple parent-child relationships should not automatically be treated as a separate module if they belong naturally to the owning domain.

### 4.4 Search

Responsible for finding and retrieving relevant knowledge across Cortexa.

Potential responsibilities:

- Text-based search.
- Filtering and sorting.
- Indexing and reindexing.
- Search result ranking.
- Integration with alternative search technologies when needed.

Search indexes should be treated as derived data when appropriate, with a clear strategy for keeping them synchronized with the source of truth.

### 4.5 AI Integration

Responsible for integrating AI capabilities with the knowledge workspace.

Potential responsibilities:

- Preparing relevant context for AI operations.
- Retrieving and assembling knowledge for a request.
- Integrating with external or local AI providers.
- Managing provider-specific request and response handling.
- Supporting future embedding-based retrieval or other AI-assisted capabilities.

AI provider implementations should remain isolated from core business logic. The application should not depend on a particular provider's SDK or API format.

### 4.6 Identity and Access Control

Responsible for authentication and the authorization mechanisms required by the product.

Potential responsibilities:

- User identity integration.
- Access policies.
- Ownership and access rules for protected resources.
- Future workspace-level or resource-level permissions when required.

Authentication mechanisms and framework-specific identity models should not become implicit dependencies throughout unrelated business modules.

---

## 5. Module Boundaries and Communication

Modules should communicate through explicit contracts that represent capabilities they are allowed to use.

The following rules are architectural guidelines:

1. A module owns its business rules and internal implementation.
2. Other modules should not access its internal repositories or persistence structures directly.
3. Cross-module operations should use exposed application contracts or another explicitly defined integration mechanism.
4. Shared contracts should remain small and purpose-specific.
5. Shared code should not become a general-purpose location for unrelated business logic.
6. Dependencies between modules should be intentional and documented.
7. Circular module dependencies should be avoided.

A module should not be created solely to hold a small number of classes. Module boundaries should follow cohesive business responsibilities.

---

## 6. Data and Persistence Strategy

Persistence decisions should preserve module ownership while avoiding unnecessary infrastructure complexity.

### 6.1 Source of Truth

The primary database should hold authoritative application data.

Search indexes, embeddings, caches, and other derived representations should not silently become competing sources of truth.

### 6.2 Database Access

Entity Framework Core may be used as the persistence technology, subject to the final technology decision.

Database-specific concerns should remain within Infrastructure.

Application code should depend on persistence abstractions only where doing so provides a useful architectural boundary.

### 6.3 Repositories and Unit of Work

Repositories should be introduced where they clarify domain-oriented persistence operations or enforce useful module boundaries.

A generic repository, custom Unit of Work, or Specification Pattern should not be adopted automatically.

Entity Framework Core already provides change tracking and a persistence coordination mechanism through `DbContext` and `SaveChangesAsync`.

Any additional abstraction should have a clearly defined responsibility, including transaction ownership, commit behavior, and error handling.

### 6.4 Transactions and Consistency

Operations that modify multiple related pieces of data must define their consistency requirements.

The design should explicitly establish:

- Which operations must be atomic.
- Where transactions begin and end.
- How failures are handled.
- How derived data is updated or rebuilt.
- How concurrent updates are handled where relevant.

Cross-module transactions should be introduced only when required by the business operation, with the consistency model documented.

### 6.5 Hierarchical Pages

A parent-child relationship is a candidate starting point for representing page hierarchies.

The implementation must account for:

- Valid parent references.
- Prevention of circular hierarchies.
- Consistent movement of pages between parents.
- Efficient retrieval of descendants when required.

More specialized hierarchy representations should be considered only if measured requirements justify them.

### 6.6 Block Content

Block persistence should separate structural information from type-specific content.

Structural information may include identifiers, ownership, block type, and position. Type-specific content may use a structured representation where appropriate.

The final design should balance schema validation, query requirements, extensibility, and database capabilities.

---

## 7. Search and AI Retrieval

Search and AI retrieval are related capabilities but should not be treated as the same responsibility.

Traditional search focuses on finding information through text matching, filters, and ranking.

AI retrieval may additionally use semantic similarity, embeddings, or context assembly.

The architecture should allow these capabilities to evolve independently while reusing shared access and retrieval contracts where appropriate.

Important design considerations include:

- Keeping source content authoritative.
- Defining how changes propagate to indexes.
- Supporting reindexing and recovery.
- Avoiding unnecessary coupling to a particular AI provider.
- Enforcing access permissions during retrieval.
- Returning enough source references to trace retrieved context back to its origin.
- Managing token and context limits when preparing information for AI requests.

AI-generated output should not automatically be treated as authoritative application data. Any operation that creates or changes persistent knowledge must follow the relevant application rules.

---

## 8. Security and Authorization

Security must be enforced at the appropriate application boundaries rather than relying exclusively on the user interface.

The architecture should account for:

- Authentication and identity management.
- Authorization for protected operations.
- Resource ownership and access checks.
- Input validation.
- Secure handling of secrets and provider credentials.
- Protection against unauthorized access through search or AI retrieval.
- Safe logging that avoids exposing sensitive content or credentials.

Search results and AI context assembly must respect the same access rules that govern direct resource access.

---

## 9. Testing Strategy

Testing should validate both individual business behavior and the integrity of architectural boundaries.

### Unit Tests

Focus on isolated business rules and application behavior without requiring external infrastructure.

### Integration Tests

Validate interactions with real infrastructure where practical, including persistence, database constraints, and external integration boundaries.

### API Tests

Validate externally observable behavior, including request validation, authorization, response contracts, and error handling.

### Architecture Tests

Validate important structural rules, such as:

- Domain code does not depend on Infrastructure.
- Application code does not depend on concrete delivery or persistence implementations.
- Modules do not access each other's internal implementation directly.
- Circular dependencies are prevented where enforceable.

Tests should focus on meaningful guarantees rather than duplicating implementation details.

---

## 10. Observability and Reliability

The application should provide enough operational visibility to diagnose failures and understand its behavior.

The initial approach should consider:

- Structured logging.
- Consistent error handling.
- Health checks for critical dependencies.
- Request and operation timing.
- Relevant application metrics.
- Correlation identifiers across related operations.
- Safe diagnostics for search and AI integrations.

More advanced observability infrastructure should be introduced in response to operational needs rather than assumed necessary from the beginning.

---

## 11. Architectural Constraints

The following constraints guide implementation decisions:

- Start with a modular monolith rather than introducing microservices without a demonstrated need.
- Keep business logic independent of infrastructure frameworks.
- Prefer explicit module contracts over direct access to another module's internals.
- Avoid speculative abstractions and unnecessary design patterns.
- Define transaction boundaries deliberately.
- Preserve a clear source of truth for persisted knowledge.
- Treat search indexes and AI-derived representations as separate concerns from authoritative content.
- Enforce authorization across APIs, search, and AI context retrieval.
- Keep architectural decisions testable and document important trade-offs.

These constraints are intended to guide decisions, not prevent justified changes when requirements or evidence evolve.

---

## 12. Open Architectural Decisions

The following decisions require validation before they should be considered final:

- **Technology stack:** Final backend, frontend, and runtime choices.
- **Database:** Relational database selection and database-specific features.
- **Persistence boundaries:** Shared database context versus separate contexts for selected modules.
- **Module structure:** Final module ownership and dependency relationships.
- **Block representation:** Schema and validation strategy for different content types.
- **Hierarchy representation:** The initial page-tree strategy and its performance characteristics.
- **Search implementation:** Initial indexing and query strategy.
- **AI retrieval:** Context assembly, semantic retrieval, and provider abstraction.
- **Identity and permissions:** Authentication implementation and authorization model.
- **Transactions:** Commit coordination and consistency requirements.
- **Background processing:** Whether indexing and AI-related work require background jobs.
- **Deployment:** Initial hosting and operational requirements.

Each decision should be made using explicit requirements, alternatives, trade-offs, and evidence where practical.

---

## 13. Architecture Decision Records

Significant architectural decisions should be recorded separately as Architecture Decision Records (ADRs).

Each ADR should contain:

- **Context:** The problem or requirement driving the decision.
- **Decision:** The chosen approach.
- **Alternatives:** The realistic options considered.
- **Consequences:** Benefits, costs, limitations, and risks.
- **Status:** Proposed, Accepted, Superseded, or Rejected.

Suggested location:

`docs/architecture/decisions/`

Suggested naming convention:

`ADR-001-modular-monolith.md`

The architecture document describes the current architectural direction. ADRs preserve the reasoning behind important decisions and provide a history of how the architecture evolves.

---

## 14. Guiding Principle

Cortexa should be designed around clear ownership, explicit dependencies, reliable data, and useful abstractions.

The goal is not to maximize the number of patterns or architectural layers. The goal is to make the system understandable, testable, and capable of evolving as its real requirements emerge.

**Architecture is a set of deliberate trade-offs, not a collection of patterns applied by default.**
