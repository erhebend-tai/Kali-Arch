Design the integration-event versioning strategy and AsyncAPI documentation
— the event-driven companion to our OpenAPI layer.

Cover:

- **Shared.Contracts versioning**: V1/V2 namespace strategy
  (Events.V1.Compute vs in-place evolution), when to bump, and how old
  consumers keep working (upcasting/mappers).
- **Schema registry**: for Kafka-compatible buses (OCI Streaming, bare
  metal) and Event Hubs schema registry — Avro/Protobuf/JSON-schema choice,
  where schemas live (Shared.Contracts as source of truth vs registry-first),
  and code generation (fits our generators philosophy).
- **AsyncAPI**: docs/asyncapi/ specs per service describing published and
  consumed channels/messages — how they're authored/generated (code-first
  like OpenAPI or hand-written), and how the gateway aggregates them.
- **Consumer migration strategy**: how a V2 event rolls out —
  dual-publish, translation handlers, sunset process.
- Relationship diagram: domain events → MediatR notifications →
  integration events (Shared.Contracts) → outbox → bus → inbox.

Standard format: mental model, tree, code, cheat sheet, connections.
