using System;
using CommandLine;
using CommandLineParser = CommandLine.Parser;

namespace DBMigrator
{
    class Program
    {
        static int Main(string[] args)
        {
            return CommandLineParser.Default.ParseArguments<GenerateScriptVerb, UpgradeDatabaseVerb>(args)
                .MapResult(
                    (GenerateScriptVerb opts) => opts.Execute(),
                    (UpgradeDatabaseVerb opts) => opts.Execute(),
                    errs => 1);
        }
    }
}
