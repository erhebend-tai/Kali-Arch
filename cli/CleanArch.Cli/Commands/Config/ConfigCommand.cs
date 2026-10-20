using System.CommandLine;

namespace CleanArch.Cli.Commands.Config;

public static class ConfigCommand
{
    public static Command Build()
    {
        var keyArgument = new Argument<string>("key")
        {
            Description = "Config key"
        };

        var get = new Command("get", "Read a config value")
        {
            keyArgument
        };
        get.SetAction((parseResult, ct) =>
        {
            var key = parseResult.GetValue(keyArgument);
            Console.WriteLine($"{key} = <value>");
            return Task.FromResult(0);
        });

        var config = new Command("config", "CLI configuration");
        config.Subcommands.Add(get);
        return config;
    }
}
