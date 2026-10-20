Add the developer-experience layer that ties the repo together.

Cover:

- `.config/dotnet-tools.json` manifest: docfx, cake.tool, dotnet-ef,
  dotnet-format, stryker — restored via `dotnet tool restore` in CI and
  devcontainer post-create.
- A root `Makefile` (or justfile) with the everyday verbs: `make build`,
  `make test`, `make analyze`, `make docs`, `make run <service>`,
  `make workers`, `make tunnel-dev`, `make scaffold` — thin wrappers over
  scaffold.sh, build.cake, docfx, and dotnet/Go/npm commands.
- Git hooks: pre-commit (dotnet format staged files + dart format +
  prettier on workers), commit-msg (conventional commits), pre-push
  (quick unit tests) — husky-style via a scripts/install-hooks.sh that
  also works on Windows.
- `.vscode/` shared debug configs: launch.json for API, Functions,
  workers service, Go services, and attach; tasks.json mapping the
  Makefile verbs.
- A root `global.json` pinning the .NET SDK, `nuget.config`, and
  `Directory.Packages.props` placement (we have it under packages/ —
  move or import?).
- Contributor flow doc: clone → codeserver/devcontainer → make bootstrap
  → first commit.

Standard format: mental model, tree, code, cheat sheet, connections.
