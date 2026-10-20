-- V020__fn_calculate_uptime.sql
-- Returns uptime percentage for a VM over a given period

CREATE OR REPLACE FUNCTION vm_service.fn_calculate_uptime(
    p_vm_id      UUID,
    p_start_date TIMESTAMPTZ,
    p_end_date   TIMESTAMPTZ
)
RETURNS NUMERIC(5,2)
LANGUAGE plpgsql
AS $$
DECLARE
    v_total_seconds  NUMERIC;
    v_running_seconds NUMERIC;
BEGIN
    v_total_seconds := EXTRACT(EPOCH FROM (p_end_date - p_start_date));

    IF v_total_seconds <= 0 THEN
        RETURN 0;
    END IF;

    SELECT COALESCE(
        SUM(
            EXTRACT(EPOCH FROM (
                LEAST(al.changed_at, p_end_date) -
                GREATEST(LAG(al.changed_at) OVER (
                    PARTITION BY al.entity_id
                    ORDER BY al.changed_at
                ), p_start_date)
            ))
        ), 0)
    INTO v_running_seconds
    FROM audit.vm_audit_log al
    WHERE al.entity_id   = p_vm_id
      AND al.entity_type = 'VirtualMachine'
      AND al.changed_at  BETWEEN p_start_date AND p_end_date
      AND al.new_values->>'status' = 'running';

    RETURN ROUND((v_running_seconds / v_total_seconds) * 100, 2);
END;
$$;
