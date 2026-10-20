using System.CommandLine;

namespace CleanArch.Cli.Commands.Provider;

public static class ProviderCommand
{
    public static Command Build()
    {
        var list = new Command("list", "List configured cloud providers");
        list.SetAction((parseResult, ct) =>
        {
            foreach (var p in new[]
                     {
                         "Azure", "AWS", "GCP", "Alibaba",
                         "Oracle", "Cloudflare", "BareMetal"
                     })
                Console.WriteLine(p);

            return Task.FromResult(0);
        });

        var provider = new Command("provider", "Cloud provider operations");
        provider.Subcommands.Add(list);
        return provider;
    }
}
