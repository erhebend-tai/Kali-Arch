-- V022__fn_tenant_vms.sql
-- Table-valued function — returns VMs for a tenant
-- Used in views and reports

CREATE OR REPLACE FUNCTION vm_service.fn_tenant_vms(
    p_tenant_id VARCHAR(100))
RETURNS SETOF vm_service.virtual_machines
LANGUAGE sql
STABLE
AS $$
    SELECT *
    FROM   vm_service.virtual_machines
    WHERE  tenant_id  = p_tenant_id
      AND  is_deleted = FALSE;
$$;
