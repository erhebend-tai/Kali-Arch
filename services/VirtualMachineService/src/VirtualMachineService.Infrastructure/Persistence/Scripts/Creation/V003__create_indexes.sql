-- V003__create_indexes.sql

-- ── Virtual Machines ──────────────────────────────────────────
CREATE INDEX CONCURRENTLY IF NOT EXISTS ix_vms_tenant_id
    ON vm_service.virtual_machines (tenant_id)
    WHERE is_deleted = FALSE;

CREATE INDEX CONCURRENTLY IF NOT EXISTS ix_vms_status
    ON vm_service.virtual_machines (status)
    WHERE is_deleted = FALSE;

CREATE INDEX CONCURRENTLY IF NOT EXISTS ix_vms_region
    ON vm_service.virtual_machines (region)
    WHERE is_deleted = FALSE;

CREATE INDEX CONCURRENTLY IF NOT EXISTS ix_vms_tenant_status
    ON vm_service.virtual_machines (tenant_id, status)
    WHERE is_deleted = FALSE;

CREATE INDEX CONCURRENTLY IF NOT EXISTS ix_vms_created_at
    ON vm_service.virtual_machines (created_at DESC);

-- Full-text search on VM name
CREATE INDEX CONCURRENTLY IF NOT EXISTS ix_vms_name_trgm
    ON vm_service.virtual_machines
    USING GIN (name gin_trgm_ops);

-- ── Outbox ────────────────────────────────────────────────────
CREATE INDEX CONCURRENTLY IF NOT EXISTS ix_outbox_pending
    ON outbox.messages (created_at ASC)
    WHERE processed_at IS NULL;

CREATE INDEX CONCURRENTLY IF NOT EXISTS ix_outbox_retry
    ON outbox.messages (next_retry_at ASC)
    WHERE processed_at IS NULL
      AND retry_count < 5;

-- ── Inbox ─────────────────────────────────────────────────────
CREATE UNIQUE INDEX CONCURRENTLY IF NOT EXISTS ix_inbox_message_id
    ON outbox.inbox_messages (message_id);

-- ── Audit ─────────────────────────────────────────────────────
CREATE INDEX CONCURRENTLY IF NOT EXISTS ix_audit_entity
    ON audit.vm_audit_log (entity_id, changed_at DESC);

CREATE INDEX CONCURRENTLY IF NOT EXISTS ix_audit_tenant
    ON audit.vm_audit_log (tenant_id, changed_at DESC);
