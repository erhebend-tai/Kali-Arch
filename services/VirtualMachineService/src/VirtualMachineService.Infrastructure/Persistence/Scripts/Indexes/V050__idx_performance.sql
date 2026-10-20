-- V050__idx_performance.sql
-- Additional performance indexes beyond the V003 baseline.
-- CONCURRENTLY — run outside a transaction in production.

-- Covering index for the hot tenant-listing query path
CREATE INDEX CONCURRENTLY IF NOT EXISTS ix_vms_tenant_created
    ON vm_service.virtual_machines (tenant_id, created_at DESC)
    INCLUDE (name, status, region, size)
    WHERE is_deleted = FALSE;

-- Snapshot lookups by VM
CREATE INDEX CONCURRENTLY IF NOT EXISTS ix_snapshots_vm
    ON vm_service.vm_snapshots (vm_id);

-- Webhook dispatch lookups
CREATE INDEX CONCURRENTLY IF NOT EXISTS ix_webhooks_tenant_active
    ON vm_service.webhook_subscriptions (tenant_id)
    WHERE is_active = TRUE;

CREATE INDEX CONCURRENTLY IF NOT EXISTS ix_webhooks_event_types
    ON vm_service.webhook_subscriptions
    USING GIN (event_types);

-- Correlation tracing across outbox and audit
CREATE INDEX CONCURRENTLY IF NOT EXISTS ix_outbox_correlation
    ON outbox.messages (correlation_id)
    WHERE correlation_id IS NOT NULL;

CREATE INDEX CONCURRENTLY IF NOT EXISTS ix_audit_correlation
    ON audit.vm_audit_log (correlation_id)
    WHERE correlation_id IS NOT NULL;
