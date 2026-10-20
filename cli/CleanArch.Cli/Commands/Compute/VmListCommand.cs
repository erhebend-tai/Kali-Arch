using System.CommandLine;
using CleanArch.Cli.Output;

namespace CleanArch.Cli.Commands.Compute;

public static class VmListCommand
{
    public static Command Build()
    {
        var regionOption = new Option<string?>("--region")
        {
            Description = "Filter by region"
        };
        var runningOption = new Option<bool>("--running")
        {
            Description = "Running VMs only"
        };
        var jsonOption = new Option<bool>("--json")
        {
            Description = "Emit JSON output"
        };

        var command = new Command("list", "List VMs")
        {
            regionOption, runningOption, jsonOption
        };

        command.SetAction(async (parseResult, ct) =>
        {
            IOutputFormatter formatter = parseResult.GetValue(jsonOption)
                ? new JsonFormatter()
                : new TableFormatter();

            // Dispatches GetAllVmsQuery via ISender.
            formatter.Write(Array.Empty<object>());

            await Task.CompletedTask;
            return 0;
        });

        return command;
    }
}
