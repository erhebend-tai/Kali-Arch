-- V002__create_tables.sql

-- ── Virtual Machines ──────────────────────────────────────────
CREATE TABLE IF NOT EXISTS vm_service.virtual_machines (
    id               UUID         NOT NULL DEFAULT uuid_generate_v4(),
    tenant_id        VARCHAR(100) NOT NULL,
    name             VARCHAR(63)  NOT NULL,
    region           VARCHAR(50)  NOT NULL,
    size             VARCHAR(20)  NOT NULL,
    os_type          VARCHAR(10)  NOT NULL,
    status           VARCHAR(20)  NOT NULL DEFAULT 'stopped',
    resource_group   VARCHAR(90)  NOT NULL,
    provider         VARCHAR(20)  NOT NULL DEFAULT 'Azure',
    created_at       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    created_by       VARCHAR(200) NOT NULL,
    updated_at       TIMESTAMPTZ,
    updated_by       VARCHAR(200),
    is_deleted       BOOLEAN      NOT NULL DEFAULT FALSE,
    deleted_at       TIMESTAMPTZ,
    deleted_by       VARCHAR(200),
    row_version      BIGINT       NOT NULL DEFAULT 0,  -- optimistic concurrency

    CONSTRAINT pk_virtual_machines PRIMARY KEY (id),
    CONSTRAINT uq_vm_name_rg_tenant
        UNIQUE (name, resource_group, tenant_id),
    CONSTRAINT chk_vm_status
        CHECK (status IN ('stopped','running','deallocated',
                          'provisioning','failed','unknown')),
    CONSTRAINT chk_vm_os_type
        CHECK (os_type IN ('Linux','Windows')),
    CONSTRAINT chk_vm_size
        CHECK (size IN ('small','medium','large','xlarge','xxlarge'))
);

-- ── VM Snapshots ──────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS vm_service.vm_snapshots (
    id               UUID         NOT NULL DEFAULT uuid_generate_v4(),
    vm_id            UUID         NOT NULL,
    tenant_id        VARCHAR(100) NOT NULL,
    name             VARCHAR(200) NOT NULL,
    size_gb          INTEGER      NOT NULL,
    created_at       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    created_by       VARCHAR(200) NOT NULL,
    expires_at       TIMESTAMPTZ,

    CONSTRAINT pk_vm_snapshots PRIMARY KEY (id),
    CONSTRAINT fk_vm_snapshots_vm
        FOREIGN KEY (vm_id)
        REFERENCES vm_service.virtual_machines (id)
        ON DELETE CASCADE
);

-- ── Webhook Subscriptions ─────────────────────────────────────
CREATE TABLE IF NOT EXISTS vm_service.webhook_subscriptions (
    id               UUID         NOT NULL DEFAULT uuid_generate_v4(),
    tenant_id        VARCHAR(100) NOT NULL,
    target_url       VARCHAR(2048) NOT NULL,
    secret           VARCHAR(256) NOT NULL,
    event_types      TEXT[]       NOT NULL,
    is_active        BOOLEAN      NOT NULL DEFAULT TRUE,
    max_retries      INTEGER      NOT NULL DEFAULT 5,
    created_at       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    last_triggered_at TIMESTAMPTZ,
    failure_count    INTEGER      NOT NULL DEFAULT 0,

    CONSTRAINT pk_webhook_subscriptions PRIMARY KEY (id),
    CONSTRAINT uq_webhook_url_tenant
        UNIQUE (tenant_id, target_url)
);

-- ── Audit Log ─────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS audit.vm_audit_log (
    id               BIGSERIAL    NOT NULL,
    entity_id        UUID         NOT NULL,
    entity_type      VARCHAR(100) NOT NULL,
    action           VARCHAR(20)  NOT NULL,  -- INSERT, UPDATE, DELETE
    old_values       JSONB,
    new_values       JSONB,
    changed_by       VARCHAR(200),
    changed_at       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    tenant_id        VARCHAR(100),
    correlation_id   VARCHAR(100),

    CONSTRAINT pk_vm_audit_log PRIMARY KEY (id)
);

-- ── Outbox ────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS outbox.messages (
    id               UUID         NOT NULL DEFAULT uuid_generate_v4(),
    topic            VARCHAR(200) NOT NULL,
    payload_type     VARCHAR(500) NOT NULL,
    payload          JSONB        NOT NULL,
    tenant_id        VARCHAR(100),
    created_at       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    processed_at     TIMESTAMPTZ,
    retry_count      INTEGER      NOT NULL DEFAULT 0,
    next_retry_at    TIMESTAMPTZ,
    error            TEXT,
    correlation_id   VARCHAR(100),

    CONSTRAINT pk_outbox_messages PRIMARY KEY (id)
);

-- ── Inbox ─────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS outbox.inbox_messages (
    id               UUID         NOT NULL DEFAULT uuid_generate_v4(),
    message_id       VARCHAR(200) NOT NULL,  -- idempotency key
    topic            VARCHAR(200) NOT NULL,
    payload_type     VARCHAR(500) NOT NULL,
    payload          JSONB        NOT NULL,
    received_at      TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    processed_at     TIMESTAMPTZ,
    retry_count      INTEGER      NOT NULL DEFAULT 0,
    error            TEXT,
    is_duplicate     BOOLEAN      NOT NULL DEFAULT FALSE,

    CONSTRAINT pk_inbox_messages PRIMARY KEY (id),
    CONSTRAINT uq_inbox_message_id UNIQUE (message_id)
);
