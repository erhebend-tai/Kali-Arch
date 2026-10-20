using System.CommandLine;

namespace CleanArch.Cli.Commands.Compute;

public static class VmCommand
{
    public static Command Build()
    {
        var vm = new Command("vm", "Virtual machine operations");

        vm.Subcommands.Add(VmProvisionCommand.Build());
        vm.Subcommands.Add(VmListCommand.Build());
        vm.Subcommands.Add(VmStartCommand.Build());

        return vm;
    }
}
