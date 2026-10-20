-- V040__trg_vm_audit_log.sql
-- Automatically logs all VM changes to audit table

CREATE OR REPLACE FUNCTION audit.fn_vm_audit_trigger()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO audit.vm_audit_log (
        entity_id,
        entity_type,
        action,
        old_values,
        new_values,
        changed_by,
        tenant_id,
        correlation_id
    ) VALUES (
        COALESCE(NEW.id, OLD.id),
        'VirtualMachine',
        TG_OP,
        CASE WHEN TG_OP = 'INSERT' THEN NULL
             ELSE to_jsonb(OLD) END,
        CASE WHEN TG_OP = 'DELETE' THEN NULL
             ELSE to_jsonb(NEW) END,
        COALESCE(NEW.updated_by, NEW.created_by, OLD.updated_by),
        COALESCE(NEW.tenant_id, OLD.tenant_id),
        current_setting('app.correlation_id', TRUE)
    );
    RETURN COALESCE(NEW, OLD);
END;
$$;

DROP TRIGGER IF EXISTS trg_vm_audit_log
    ON vm_service.virtual_machines;

CREATE TRIGGER trg_vm_audit_log
    AFTER INSERT OR UPDATE OR DELETE
    ON vm_service.virtual_machines
    FOR EACH ROW
    EXECUTE FUNCTION audit.fn_vm_audit_trigger();
