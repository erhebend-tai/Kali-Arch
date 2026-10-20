Design the Inbox pattern and idempotency story — the complement to the
existing Outbox (Infrastructure.Common/Outbox).

Cover:

- `InboxMessage` entity/table alongside OutboxMessage: consumer dedupe so a
  delivered-twice event processes exactly once. Where it lives in
  Infrastructure.Common/Outbox vs a sibling `Inbox/` folder.
- The consume path: Dapr subscriber → store inbox record (unique by
  messageId+consumer) → dispatch → mark processed; how this wraps MediatR
  notification handlers.
- `IdempotentCommand` handling on the write side: `Idempotency-Key` header
  support in the API, key storage, replay-safe command execution, and how it
  composes with TransactionBehavior and Unit of Work.
- An `InboxProcessor` / cleanup worker for the .NET Worker Services project
  (workers/CleanArch.Workers) paralleling OutboxProcessor.
- End-to-end message flow diagram: publisher → outbox → bus → inbox →
  handler, including failure/retry/DLQ branches for both directions.

Standard format: mental model, tree, code, cheat sheet, connections.
