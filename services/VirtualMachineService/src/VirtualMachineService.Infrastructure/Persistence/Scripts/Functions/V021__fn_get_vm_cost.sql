-- V021__fn_get_vm_cost.sql
-- Estimated cost of a VM over a period — size rate × uptime hours

CREATE OR REPLACE FUNCTION vm_service.fn_get_vm_cost(
    p_vm_id      UUID,
    p_start_date TIMESTAMPTZ,
    p_end_date   TIMESTAMPTZ
)
RETURNS NUMERIC(10,2)
LANGUAGE plpgsql
AS $$
DECLARE
    v_size          VARCHAR(20);
    v_hourly_rate   NUMERIC(10,4);
    v_uptime_pct    NUMERIC(5,2);
    v_period_hours  NUMERIC;
BEGIN
    SELECT size
    INTO   v_size
    FROM   vm_service.virtual_machines
    WHERE  id = p_vm_id;

    IF v_size IS NULL THEN
        RETURN 0;
    END IF;

    -- Hourly rates — keep in sync with V102__seed_vm_sizes.sql
    v_hourly_rate := CASE v_size
        WHEN 'small'   THEN 0.020
        WHEN 'medium'  THEN 0.050
        WHEN 'large'   THEN 0.100
        WHEN 'xlarge'  THEN 0.200
        WHEN 'xxlarge' THEN 0.400
        ELSE 0
    END;

    v_period_hours :=
        EXTRACT(EPOCH FROM (p_end_date - p_start_date)) / 3600;

    IF v_period_hours <= 0 THEN
        RETURN 0;
    END IF;

    v_uptime_pct := vm_service.fn_calculate_uptime(
        p_vm_id, p_start_date, p_end_date);

    -- Cost = full-period cost prorated by observed uptime
    RETURN ROUND(
        v_period_hours * v_hourly_rate * (v_uptime_pct / 100), 2);
END;
$$;
