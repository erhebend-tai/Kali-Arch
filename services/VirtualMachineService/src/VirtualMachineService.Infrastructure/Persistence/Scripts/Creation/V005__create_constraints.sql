-- V005__create_constraints.sql
-- Constraints that can't be declared inline: cross-table checks
-- and non-negative guards on the messaging tables.

ALTER TABLE outbox.messages
    ADD CONSTRAINT chk_outbox_retry_count
    CHECK (retry_count >= 0);

ALTER TABLE outbox.inbox_messages
    ADD CONSTRAINT chk_inbox_retry_count
    CHECK (retry_count >= 0);

ALTER TABLE vm_service.vm_snapshots
    ADD CONSTRAINT chk_snapshot_size
    CHECK (size_gb > 0);

ALTER TABLE vm_service.webhook_subscriptions
    ADD CONSTRAINT chk_webhook_max_retries
    CHECK (max_retries >= 0),
    ADD CONSTRAINT chk_webhook_event_types
    CHECK (array_length(event_types, 1) > 0);
