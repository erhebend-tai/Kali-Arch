Add the alternate protocol surfaces beyond REST — SignalR, gRPC, and
optionally GraphQL — all reusing the same Application layer.

Cover:

- **SignalR**: real-time push to the Blazor/Flutter clients — hub
  placement in the API project (Hubs/ alongside Controllers/), typed hub
  clients, scaling out (Azure SignalR Service vs Redis backplane for bare
  metal), and how domain events reach connected clients (notification
  handler → hub).
- **gRPC**: service-to-service calls as an alternative to REST+Dapr —
  .proto files location (shared/protos/ vs Shared.Contracts), code-gen in
  build, gRPC services calling ISender (same MediatR pipeline as
  controllers), and gateway transcoding vs direct routing through YARP.
- **GraphQL** (HotChocolate): an optional query layer over
  Queries/ — where it lives, whether it competes with or complements
  REST, and filtering/projection onto Specifications.
- **Tunnel + gateway routing** for all three: cloudflared ingress rules,
  YARP config, and what changes in deploy/.
- Decision matrix: when to use REST vs SignalR vs gRPC vs GraphQL vs
  message bus for a given interaction.

Standard format: mental model, tree, code, cheat sheet, connections.
