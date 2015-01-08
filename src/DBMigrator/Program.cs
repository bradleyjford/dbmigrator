using System;

using CommandLineParser = CommandLine.Parser;

namespace DBMigrator
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();

            var invokedVerb = "script";
            CommandLineVerb invokedVerbOptions = null;

            if (!CommandLineParser.Default.ParseArgumentsStrict(args, options, (v, o) =>
            {
                invokedVerb = v;
                invokedVerbOptions = (CommandLineVerb)o;
            }))
            {
                Environment.Exit(CommandLineParser.DefaultExitCodeFail);
            }

            if (invokedVerbOptions == null)
            {
                Environment.Exit(CommandLineParser.DefaultExitCodeFail);
            }

            invokedVerbOptions.Execute();
        }
    }
}
