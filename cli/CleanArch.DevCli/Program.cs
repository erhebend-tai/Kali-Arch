using System.CommandLine;
using CleanArch.DevCli.Commands.Scaffold;

var root = new RootCommand(
    "CleanArch developer CLI — scaffolding, codegen, analysis tasks.");

root.Subcommands.Add(ScaffoldCommand.Build());

return await root.Parse(args).InvokeAsync();
