using System.CommandLine;
using System.Diagnostics;

namespace CleanArch.DevCli.Commands.Scaffold;

/// <summary>
/// Scaffolds a new mini Clean Architecture microservice —
/// drives `dotnet new classlib` per layer plus project refs.
/// </summary>
public static class ScaffoldCommand
{
    public static Command Build()
    {
        var nameArg = new Argument<string>("name")
        {
            Description = "Service name (e.g. PaymentService)"
        };
        var pathOption = new Option<string>("--path")
        {
            Description = "Output root",
            DefaultValueFactory = _ => "services"
        };

        var command = new Command("service",
            "Scaffold a new microservice solution")
        {
            nameArg, pathOption
        };

        command.SetAction(async (parseResult, ct) =>
        {
            var name = parseResult.GetValue(nameArg)!;
            var root = Path.Combine(
                parseResult.GetValue(pathOption)!, name);

            foreach (var layer in new[]
                     { "Domain", "Application", "Infrastructure" })
            {
                var project = Path.Combine(root, "src",
                    $"{name}.{layer}", $"{name}.{layer}.csproj");

                Console.WriteLine($"Creating {name}.{layer}...");
                await RunDotnetAsync(
                    $"new classlib -n {name}.{layer} " +
                    $"-o \"{Path.GetDirectoryName(project)}\" " +
                    $"-f net10.0");
            }

            Console.WriteLine($"Done — add projects to CleanArch.slnx.");
            return 0;
        });

        return command;
    }

    private static async Task RunDotnetAsync(string arguments)
    {
        var psi = new ProcessStartInfo("dotnet", arguments)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        using var process = Process.Start(psi)!;
        await process.WaitForExitAsync();
    }
}
