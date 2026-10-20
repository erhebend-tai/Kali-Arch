Design the complete observability layer beyond the Serilog folders we have.

Cover:

- **OpenTelemetry** setup: where does the OTel registration live —
  Shared.Infrastructure/Observability or Infrastructure.Common/Observability?
  Traces + metrics + logs with a single AddPlatformObservability() extension
  used by API, Functions, Workers, and MCP servers.
- **Per-provider exporters**: Azure Monitor/App Insights, CloudWatch X-Ray,
  Cloud Trace, plus bare-metal (Prometheus + Loki + Tempo/Jaeger) and how
  the CloudProvider switch selects them.
- **Golden signals instrumentation**: HTTP/gRPC server spans, outbound
  HttpClient + Dapr spans, MediatR pipeline spans (a TracingBehavior?),
  EF Core spans, custom metrics per service.
- **Dashboards + alerts as code**: a top-level `observability/` folder with
  Grafana dashboard JSON, Prometheus alert rules, SLO/SLI definitions per
  service, and how they deploy (ConfigMaps / terraform).
- **Health checks**: readiness vs liveness endpoints per service, k8s probe
  wiring, deep checks (db, message bus, provider SDK) tagged appropriately.
- Correlation: W3C traceparent through HTTP, Dapr pub/sub, and the
  cloudflared tunnel; CorrelationIdEnricher alignment.

Standard format: mental model, tree, code, cheat sheet, connections.
