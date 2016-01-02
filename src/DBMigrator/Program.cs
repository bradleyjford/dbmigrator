using System;
using System.Linq;
using CommandLine;
using CommandLine.Text;
using CommandLineParser = CommandLine.Parser;

namespace DBMigrator
{
    class Program
    {
        static void Main(string[] args)
        {
            var parserResult = CommandLineParser.Default.ParseArguments(args, typeof(GenerateScriptVerb), typeof(UpgradeDatabaseVerb));
            
            if (parserResult.Errors.Any())
            {
                Environment.Exit(-1);
            }

            WriteLogo(parserResult);

            var handler = (IVerbHandler)parserResult.Value;

            handler.Execute();
        }

        static void WriteLogo<T>(ParserResult<T> parserResult)
        {
            var helpText = HelpText.AutoBuild(parserResult);

            Console.WriteLine("{0}{2}{1}{2}", helpText.Heading, helpText.Copyright, Environment.NewLine);
        }
    }
}
