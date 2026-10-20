Add a strongly-typed configuration layer so `config["Azure:OSS:Endpoint"]`
string-indexing disappears.

Cover:

- An `Options/` convention — where do the option classes live? Application
  (cloud-neutral option shapes) vs each Infrastructure.{provider} project
  (provider-specific options) vs Shared. Recommend the split and justify.
- `IOptions<T>` vs `IOptionsSnapshot<T>` vs `IOptionsMonitor<T>` — which
  where, given config hot-reload via our FileWatcher (ConfigFileWatcher).
- Validation at startup: data annotations vs `IValidateOptions<T>` vs
  `ValidateOnStart()` — fail-fast on missing secrets before any request.
- How appsettings.json sections map per provider
  (`"Alibaba": { "OSS": {...} }`) and how the provider switch reads the
  right section.
- Secrets never in appsettings: how options bind references
  (vault paths / env var names) resolved through ISecretStoreService.
- Folder tree, DI extension shape (`services.AddAzureOptions(config)`),
  and a cheat sheet.

Standard format: mental model, tree, code, cheat sheet, connections.
