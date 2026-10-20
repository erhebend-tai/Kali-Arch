-- V010__sp_provision_vm.sql
-- Atomically provisions a VM and writes to outbox

CREATE OR REPLACE PROCEDURE vm_service.sp_provision_vm(
    p_id             UUID,
    p_tenant_id      VARCHAR(100),
    p_name           VARCHAR(63),
    p_region         VARCHAR(50),
    p_size           VARCHAR(20),
    p_os_type        VARCHAR(10),
    p_resource_group VARCHAR(90),
    p_provider       VARCHAR(20),
    p_created_by     VARCHAR(200),
    p_correlation_id VARCHAR(100)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_vm_count INTEGER;
BEGIN
    -- Check tenant quota
    SELECT COUNT(*)
    INTO   v_vm_count
    FROM   vm_service.virtual_machines
    WHERE  tenant_id  = p_tenant_id
      AND  is_deleted = FALSE;

    IF v_vm_count >= 50 THEN
        RAISE EXCEPTION 'QUOTA_EXCEEDED: Tenant % has reached VM limit of 50',
            p_tenant_id;
    END IF;

    -- Check for duplicate name in resource group
    IF EXISTS (
        SELECT 1
        FROM   vm_service.virtual_machines
        WHERE  name           = p_name
          AND  resource_group = p_resource_group
          AND  tenant_id      = p_tenant_id
          AND  is_deleted     = FALSE
    ) THEN
        RAISE EXCEPTION 'DUPLICATE_VM: VM % already exists in resource group %',
            p_name, p_resource_group;
    END IF;

    -- Insert VM
    INSERT INTO vm_service.virtual_machines (
        id, tenant_id, name, region, size,
        os_type, status, resource_group,
        provider, created_by
    ) VALUES (
        p_id, p_tenant_id, p_name, p_region, p_size,
        p_os_type, 'provisioning', p_resource_group,
        p_provider, p_created_by
    );

    -- Write to outbox atomically (same transaction)
    INSERT INTO outbox.messages (
        topic, payload_type, payload,
        tenant_id, correlation_id
    ) VALUES (
        'vm-provisioned',
        'VmProvisionedIntegrationEvent',
        jsonb_build_object(
            'virtualMachineId', p_id,
            'name',             p_name,
            'region',           p_region,
            'size',             p_size,
            'osType',           p_os_type,
            'tenantId',         p_tenant_id,
            'provisionedBy',    p_created_by,
            'occurredAt',       NOW()
        ),
        p_tenant_id,
        p_correlation_id
    );

    COMMIT;
END;
$$;
