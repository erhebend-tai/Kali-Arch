-- V041__trg_vm_soft_delete.sql
-- Prevents hard deletes — converts DELETE to soft delete

CREATE OR REPLACE FUNCTION vm_service.fn_prevent_hard_delete()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
    -- Convert hard delete to soft delete
    UPDATE vm_service.virtual_machines
    SET    is_deleted = TRUE,
           deleted_at = NOW(),
           deleted_by = current_setting('app.current_user', TRUE)
    WHERE  id = OLD.id;

    RETURN NULL; -- Cancel the actual DELETE
END;
$$;

DROP TRIGGER IF EXISTS trg_vm_soft_delete
    ON vm_service.virtual_machines;

CREATE TRIGGER trg_vm_soft_delete
    BEFORE DELETE
    ON vm_service.virtual_machines
    FOR EACH ROW
    WHEN (OLD.is_deleted = FALSE)
    EXECUTE FUNCTION vm_service.fn_prevent_hard_delete();
