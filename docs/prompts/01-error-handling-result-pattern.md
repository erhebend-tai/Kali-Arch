Add a functional error-handling layer to the platform.

I want a `Result<T>` / `Result` + `Error` discriminated pattern instead of
throwing exceptions for expected failures (validation, not-found, conflict,
quota-exceeded). Cover:

- Where `Result<T>`, `Error`, and `ErrorType` (Validation/NotFound/Conflict/
  Failure/Unauthorized) live — Shared.Domain vs Application/Common, and why.
- How the MediatR pipeline converts domain exceptions into Results
  (an ExceptionHandlingBehavior) vs when handlers return Results directly.
- A `ResultExtensions` / endpoint mapper that converts Result → IActionResult
  / ProblemDetails in the API and Functions triggers — one canonical mapping,
  not per-controller logic.
- How domain rules signal failure: guard clauses in value objects throwing
  domain exceptions vs returning Results — pick one convention and justify it.
- How Results flow through the Step/Pipeline pattern (StepResult already
  exists — unify or keep separate?).
- Folder placement in Domain/Application/Shared + updated DI/pipeline order.

Follow the established format: mental model first, folder structure tree,
representative code, a "Where does it go?" cheat sheet, and how it connects
to the existing MediatR behaviors and ValidationBehavior.
