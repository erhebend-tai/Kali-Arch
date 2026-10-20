-- V101__seed_regions.sql
-- Supported deployment regions

CREATE TABLE IF NOT EXISTS vm_service.regions (
    name         VARCHAR(50)  PRIMARY KEY,
    display_name VARCHAR(100) NOT NULL,
    provider     VARCHAR(20)  NOT NULL
);

INSERT INTO vm_service.regions (name, display_name, provider) VALUES
    ('eastus',        'East US',         'Azure'),
    ('eastus2',       'East US 2',       'Azure'),
    ('westus2',       'West US 2',       'Azure'),
    ('westeurope',    'West Europe',     'Azure'),
    ('uksouth',       'UK South',        'Azure'),
    ('us-east-1',     'N. Virginia',     'AWS'),
    ('us-west-2',     'Oregon',          'AWS'),
    ('eu-west-1',     'Ireland',         'AWS'),
    ('us-central1',   'Iowa',            'GCP'),
    ('europe-west1',  'Belgium',         'GCP'),
    ('bare-metal-1',  'On-prem rack A',  'BareMetal')
ON CONFLICT (name) DO NOTHING;
