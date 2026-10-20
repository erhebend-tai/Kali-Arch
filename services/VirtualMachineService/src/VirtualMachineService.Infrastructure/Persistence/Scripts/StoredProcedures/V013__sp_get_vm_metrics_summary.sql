-- V013__sp_get_vm_metrics_summary.sql
-- Per-tenant fleet summary — powers the dashboard metrics widget

CREATE OR REPLACE FUNCTION vm_service.sp_get_vm_metrics_summary(
    p_tenant_id VARCHAR(100)
)
RETURNS TABLE (
    total_vms        BIGINT,
    running_vms      BIGINT,
    stopped_vms      BIGINT,
    failed_vms       BIGINT,
    provisioning_vms BIGINT,
    snapshot_count   BIGINT,
    total_disk_gb    BIGINT,
    oldest_vm_at     TIMESTAMPTZ,
    newest_vm_at     TIMESTAMPTZ
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT
        COUNT(*),
        COUNT(*) FILTER (WHERE vm.status = 'running'),
        COUNT(*) FILTER (WHERE vm.status = 'stopped'),
        COUNT(*) FILTER (WHERE vm.status = 'failed'),
        COUNT(*) FILTER (WHERE vm.status = 'provisioning'),
        COALESCE(snap.cnt, 0),
        COALESCE(snap.gb, 0),
        MIN(vm.created_at),
        MAX(vm.created_at)
    FROM vm_service.virtual_machines vm
    LEFT JOIN LATERAL (
        SELECT COUNT(*) AS cnt, SUM(s.size_gb) AS gb
        FROM   vm_service.vm_snapshots s
        WHERE  s.tenant_id = vm.tenant_id
    ) snap ON TRUE
    WHERE vm.tenant_id  = p_tenant_id
      AND vm.is_deleted = FALSE
    GROUP BY snap.cnt, snap.gb;
END;
$$;
