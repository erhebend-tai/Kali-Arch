using System.CommandLine;

namespace CleanArch.Cli.Commands.Compute;

public static class VmStartCommand
{
    public static Command Build()
    {
        var idArgument = new Argument<Guid>("id")
        {
            Description = "VM id"
        };

        var command = new Command("start", "Start a VM")
        {
            idArgument
        };

        command.SetAction(async (parseResult, ct) =>
        {
            var id = parseResult.GetValue(idArgument);
            Console.WriteLine($"StartVmCommand({id}) — dispatched via ISender.");
            await Task.CompletedTask;
            return 0;
        });

        return command;
    }
}
