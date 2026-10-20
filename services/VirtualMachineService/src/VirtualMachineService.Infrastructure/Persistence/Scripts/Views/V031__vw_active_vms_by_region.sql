-- V031__vw_active_vms_by_region.sql
-- Fleet rollup per region/provider — read model

CREATE OR REPLACE VIEW vm_service.vw_active_vms_by_region AS
SELECT
    region,
    provider,
    COUNT(*)                                    AS total_vms,
    COUNT(*) FILTER (WHERE status = 'running')  AS running_vms,
    COUNT(*) FILTER (WHERE status = 'stopped')  AS stopped_vms,
    COUNT(*) FILTER (WHERE status = 'failed')   AS failed_vms,
    COUNT(DISTINCT tenant_id)                   AS tenant_count
FROM vm_service.virtual_machines
WHERE is_deleted = FALSE
GROUP BY region, provider;
