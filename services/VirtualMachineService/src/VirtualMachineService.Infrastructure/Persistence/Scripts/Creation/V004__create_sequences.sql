-- V004__create_sequences.sql

-- Sequences for human-readable IDs
CREATE SEQUENCE IF NOT EXISTS vm_service.vm_number_seq
    START WITH 1000
    INCREMENT BY 1
    NO MAXVALUE
    CACHE 10;

-- Usage: 'VM-' || LPAD(nextval('vm_service.vm_number_seq')::TEXT, 6, '0')
-- Produces: VM-001000, VM-001001, ...
