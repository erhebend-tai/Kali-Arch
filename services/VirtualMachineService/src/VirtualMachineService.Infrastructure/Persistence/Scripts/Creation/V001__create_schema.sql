-- V001__create_schema.sql
-- Creates the platform schema and extensions

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";              -- trigram search
CREATE EXTENSION IF NOT EXISTS "btree_gin";            -- GIN index support
CREATE EXTENSION IF NOT EXISTS "pg_stat_statements";   -- query analysis

-- Dedicated schema per service (schema isolation strategy)
CREATE SCHEMA IF NOT EXISTS vm_service;
CREATE SCHEMA IF NOT EXISTS identity_service;
CREATE SCHEMA IF NOT EXISTS storage_service;
CREATE SCHEMA IF NOT EXISTS ai_service;
CREATE SCHEMA IF NOT EXISTS audit;
CREATE SCHEMA IF NOT EXISTS outbox;

-- Set search path
SET search_path TO vm_service, public;
