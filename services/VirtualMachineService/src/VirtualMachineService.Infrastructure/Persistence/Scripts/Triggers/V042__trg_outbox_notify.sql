-- V042__trg_outbox_notify.sql
-- LISTEN/NOTIFY for real-time outbox processing
-- Wakes up the outbox processor immediately on new message

CREATE OR REPLACE FUNCTION outbox.fn_notify_new_message()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
    PERFORM pg_notify(
        'outbox_new_message',
        json_build_object(
            'id',    NEW.id,
            'topic', NEW.topic
        )::TEXT
    );
    RETURN NEW;
END;
$$;

DROP TRIGGER IF EXISTS trg_outbox_notify
    ON outbox.messages;

CREATE TRIGGER trg_outbox_notify
    AFTER INSERT
    ON outbox.messages
    FOR EACH ROW
    EXECUTE FUNCTION outbox.fn_notify_new_message();
