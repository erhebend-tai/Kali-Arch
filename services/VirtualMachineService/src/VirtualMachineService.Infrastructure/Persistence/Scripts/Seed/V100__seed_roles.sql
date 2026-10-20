-- V100__seed_roles.sql
-- Reference roles used by authorization policies

CREATE TABLE IF NOT EXISTS identity_service.roles (
    name        VARCHAR(50)  PRIMARY KEY,
    description VARCHAR(200) NOT NULL
);

INSERT INTO identity_service.roles (name, description) VALUES
    ('admin',    'Full access to all tenant resources'),
    ('operator', 'Manage VMs — start, stop, resize, provision'),
    ('viewer',   'Read-only access to VM inventory'),
    ('billing',  'Read access to usage and cost data')
ON CONFLICT (name) DO NOTHING;
