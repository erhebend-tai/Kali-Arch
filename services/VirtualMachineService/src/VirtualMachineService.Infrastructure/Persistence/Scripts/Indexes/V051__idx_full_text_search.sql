-- V051__idx_full_text_search.sql
-- Trigram + JSONB search indexes. CONCURRENTLY — run outside
-- a transaction in production.

-- Fuzzy search on resource group alongside the name index (V003)
CREATE INDEX CONCURRENTLY IF NOT EXISTS ix_vms_resource_group_trgm
    ON vm_service.virtual_machines
    USING GIN (resource_group gin_trgm_ops)
    WHERE is_deleted = FALSE;

-- Search inside outbox payloads (e.g. find events by inner keys)
CREATE INDEX CONCURRENTLY IF NOT EXISTS ix_outbox_payload_gin
    ON outbox.messages
    USING GIN (payload jsonb_path_ops);

-- Search inside audit payloads (old/new row JSON)
CREATE INDEX CONCURRENTLY IF NOT EXISTS ix_audit_new_values_gin
    ON audit.vm_audit_log
    USING GIN (new_values jsonb_path_ops);
