-- V012__sp_cleanup_stale_vms.sql
-- Soft-deletes VMs stopped for more than N days
-- Called by maintenance worker on schedule

CREATE OR REPLACE PROCEDURE vm_service.sp_cleanup_stale_vms(
    p_idle_days      INTEGER DEFAULT 30,
    p_dry_run        BOOLEAN DEFAULT TRUE,
    OUT p_deleted_count INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF p_dry_run THEN
        -- Count only, no changes
        SELECT COUNT(*)
        INTO   p_deleted_count
        FROM   vm_service.virtual_machines
        WHERE  status      = 'stopped'
          AND  is_deleted  = FALSE
          AND  updated_at  < NOW() - (p_idle_days || ' days')::INTERVAL;

        RAISE NOTICE 'DRY RUN: Would delete % stale VMs', p_deleted_count;
    ELSE
        -- Soft delete
        WITH deleted AS (
            UPDATE vm_service.virtual_machines
            SET    is_deleted  = TRUE,
                   deleted_at  = NOW(),
                   deleted_by  = 'system:cleanup'
            WHERE  status      = 'stopped'
              AND  is_deleted  = FALSE
              AND  updated_at  < NOW() - (p_idle_days || ' days')::INTERVAL
            RETURNING id
        )
        SELECT COUNT(*) INTO p_deleted_count FROM deleted;

        RAISE NOTICE 'Deleted % stale VMs', p_deleted_count;
        COMMIT;
    END IF;
END;
$$;
