-- V032__vw_tenant_resource_usage.sql
-- Per-tenant fleet rollup — billing and quota dashboards

CREATE OR REPLACE VIEW vm_service.vw_tenant_resource_usage AS
SELECT
    tenant_id,
    COUNT(*)                                    AS total_vms,
    COUNT(*) FILTER (WHERE status = 'running')  AS running_vms,
    COUNT(*) FILTER (WHERE size = 'small')      AS small_vms,
    COUNT(*) FILTER (WHERE size = 'medium')     AS medium_vms,
    COUNT(*) FILTER (WHERE size = 'large')      AS large_vms,
    COUNT(*) FILTER (WHERE size = 'xlarge')     AS xlarge_vms,
    COUNT(*) FILTER (WHERE size = 'xxlarge')    AS xxlarge_vms,
    MIN(created_at)                             AS first_vm_at,
    MAX(created_at)                             AS last_vm_at
FROM vm_service.virtual_machines
WHERE is_deleted = FALSE
GROUP BY tenant_id;
