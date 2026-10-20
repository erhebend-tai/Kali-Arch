-- V102__seed_vm_sizes.sql
-- Size catalog — hourly rates keep fn_get_vm_cost in sync

CREATE TABLE IF NOT EXISTS vm_service.vm_sizes (
    name         VARCHAR(20)   PRIMARY KEY,
    vcpus        INTEGER       NOT NULL,
    memory_gb    INTEGER       NOT NULL,
    hourly_rate  NUMERIC(10,4) NOT NULL
);

INSERT INTO vm_service.vm_sizes
    (name, vcpus, memory_gb, hourly_rate) VALUES
    ('small',   1,  2,   0.020),
    ('medium',  2,  4,   0.050),
    ('large',   4,  16,  0.100),
    ('xlarge',  8,  32,  0.200),
    ('xxlarge', 16, 64,  0.400)
ON CONFLICT (name) DO NOTHING;
