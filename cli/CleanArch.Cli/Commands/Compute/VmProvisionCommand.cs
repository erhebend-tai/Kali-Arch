using System.CommandLine;
using CleanArch.Cli.Output;

namespace CleanArch.Cli.Commands.Compute;

public static class VmProvisionCommand
{
    public static Command Build()
    {
        var nameOption = new Option<string>("--name")
        {
            Description = "VM name (lowercase alphanumeric + hyphens)",
            Required = true
        };
        var rgOption = new Option<string>("--resource-group", "--rg")
        {
            Description = "Target resource group",
            Required = true
        };
        var regionOption = new Option<string>("--region")
        {
            Description = "Target region",
            Required = true
        };
        var sizeOption = new Option<string>("--size")
        {
            Description = "Machine size",
            DefaultValueFactory = _ => "medium"
        };
        var osOption = new Option<string>("--os")
        {
            Description = "OS type (Linux|Windows)",
            DefaultValueFactory = _ => "Linux"
        };
        var jsonOption = new Option<bool>("--json")
        {
            Description = "Emit JSON output"
        };

        var command = new Command("provision", "Provision a VM")
        {
            nameOption, rgOption, regionOption, sizeOption, osOption, jsonOption
        };

        command.SetAction(async (parseResult, ct) =>
        {
            IOutputFormatter formatter = parseResult.GetValue(jsonOption)
                ? new JsonFormatter()
                : new TableFormatter();

            // Entry point → same ProvisionVmCommand the API sends
            // via ISender. Wire ISender through DI in a hosted CLI.
            formatter.Write(new
            {
                Name = parseResult.GetValue(nameOption),
                ResourceGroup = parseResult.GetValue(rgOption),
                Region = parseResult.GetValue(regionOption),
                Size = parseResult.GetValue(sizeOption),
                Os = parseResult.GetValue(osOption),
                Action = "provision (dispatch via ISender)"
            });

            await Task.CompletedTask;
            return 0;
        });

        return command;
    }
}
