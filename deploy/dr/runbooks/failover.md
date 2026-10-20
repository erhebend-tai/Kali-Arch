# Disaster Recovery — Failover Runbook

## RPO / RTO targets

| Environment | RPO   | RTO    |
|-------------|-------|--------|
| production  | 5 min | 30 min |
| staging     | 1 hr  | 4 hr   |

## Trigger

Failover is invoked when the primary region is unreachable for
**15 minutes** or the error budget is exhausted.

## Steps

1. **Confirm** — verify the outage isn't a single-zone event.
   `kubectl get nodes` + provider status page.
2. **Database** — promote the standby replica.
   - Azure: `az sql db replica set-primary`
   - AWS: `aws rds promote-read-replica`
   - Verify write connectivity from the DR region.
3. **DNS** — shift traffic via the Cloudflare load balancer
   (low-TTL pool) or provider-native traffic manager.
4. **Secrets** — confirm the DR region's secret store is
   hydrated (replication runs continuously; validate
   `SecretRotationService` reports healthy).
5. **Scale** — raise DR replicas to production levels.
6. **Verify** — `/healthz/ready` green, k6 smoke scenario
   (`tests/performance/k6`) at 10% load.
7. **Declare** — post to #incidents; open a postmortem issue.

## Rollback

Reverse 3→2→1 once the primary region passes the same checks.
Writes during the failover window replay through the inbox
(`Infrastructure.Common/Inbox`) — consumers dedupe by EventId.
