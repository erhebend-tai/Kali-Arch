-- V011__sp_get_vms_by_tenant.sql
-- Paginated VM list with filtering

CREATE OR REPLACE FUNCTION vm_service.sp_get_vms_by_tenant(
    p_tenant_id  VARCHAR(100),
    p_status     VARCHAR(20)  DEFAULT NULL,
    p_region     VARCHAR(50)  DEFAULT NULL,
    p_search     VARCHAR(200) DEFAULT NULL,
    p_page       INTEGER      DEFAULT 1,
    p_page_size  INTEGER      DEFAULT 20
)
RETURNS TABLE (
    id             UUID,
    name           VARCHAR,
    region         VARCHAR,
    size           VARCHAR,
    os_type        VARCHAR,
    status         VARCHAR,
    resource_group VARCHAR,
    created_at     TIMESTAMPTZ,
    total_count    BIGINT
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT
        vm.id,
        vm.name,
        vm.region,
        vm.size,
        vm.os_type,
        vm.status,
        vm.resource_group,
        vm.created_at,
        COUNT(*) OVER() AS total_count
    FROM vm_service.virtual_machines vm
    WHERE vm.tenant_id  = p_tenant_id
      AND vm.is_deleted = FALSE
      AND (p_status IS NULL OR vm.status = p_status)
      AND (p_region IS NULL OR vm.region = p_region)
      AND (p_search IS NULL OR vm.name ILIKE '%' || p_search || '%')
    ORDER BY vm.created_at DESC
    LIMIT  p_page_size
    OFFSET (p_page - 1) * p_page_size;
END;
$$;
