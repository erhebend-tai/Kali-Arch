using System.CommandLine;
using CleanArch.Cli.Commands.Compute;
using CleanArch.Cli.Commands.Config;
using CleanArch.Cli.Commands.Provider;

var root = new RootCommand(
    "CleanArch platform operator CLI — same use cases, CLI-shaped entry point.");

root.Subcommands.Add(VmCommand.Build());
root.Subcommands.Add(ConfigCommand.Build());
root.Subcommands.Add(ProviderCommand.Build());

return await root.Parse(args).InvokeAsync();
