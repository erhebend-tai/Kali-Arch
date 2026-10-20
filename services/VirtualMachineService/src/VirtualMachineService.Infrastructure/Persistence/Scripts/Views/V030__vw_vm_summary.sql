-- V030__vw_vm_summary.sql
-- Denormalized read model for dashboard queries

CREATE OR REPLACE VIEW vm_service.vw_vm_summary AS
SELECT
    vm.id,
    vm.tenant_id,
    vm.name,
    vm.region,
    vm.size,
    vm.os_type,
    vm.status,
    vm.resource_group,
    vm.provider,
    vm.created_at,
    vm.created_by,
    vm.updated_at,
    -- Computed fields
    CASE vm.status
        WHEN 'running' THEN TRUE
        ELSE FALSE
    END AS is_running,
    EXTRACT(EPOCH FROM (NOW() - vm.created_at)) / 3600 AS age_hours,
    vm_service.fn_calculate_uptime(
        vm.id,
        NOW() - INTERVAL '30 days',
        NOW()
    ) AS uptime_30d_pct
FROM vm_service.virtual_machines vm
WHERE vm.is_deleted = FALSE;
