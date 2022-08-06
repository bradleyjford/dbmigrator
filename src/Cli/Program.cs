using CommandLine;
using CommandLineParser = CommandLine.Parser;

namespace DbMigrator.Cli;

static class Program
{
    static async Task<int> Main(string[] args)
    {
        return await CommandLineParser.Default.ParseArguments<GenerateScriptVerb, UpgradeDatabaseVerb>(args)
            .MapResult(
                async (GenerateScriptVerb opts) => await opts.ExecuteAsync(),
                async (UpgradeDatabaseVerb opts) => await opts.ExecuteAsync(),
                errs => Task.FromResult(1));
    }
}