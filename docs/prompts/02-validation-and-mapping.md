Add the validation and object-mapping layers that ValidationBehavior and the
CQRS commands need.

Two related pieces:

1. **FluentValidation** — an `Application/Validators/<domain>/` folder
   convention, one `IValidator<T>` per command/query, auto-registration via
   assembly scanning, how ValidationBehavior resolves them, custom rule
   extensions (e.g., `MustBeResourceName()` reusing domain value-object
   rules), and how validation errors surface as ErrorType.Validation results.

2. **Object mapping** — DTOs already exist in Application/DTOs. Add the
   mapping layer: Mapster vs AutoMapper (recommend one, justify),
   `Application/Mappings/<domain>/*Profile.cs`, where request → command →
   entity → DTO mapping happens (handler vs dedicated mapper interface vs
   generated), and compile-time-safe mapping (source-generated mappers)
   fitting our generators philosophy.

Cover folder trees for both monolith and microservice Application projects,
DI registration, and where validators/mappers sit relative to Commands/,
Queries/, and UseCases/ (which should go away or stay?).

Standard format: mental model, tree, code, cheat sheet, connections.
