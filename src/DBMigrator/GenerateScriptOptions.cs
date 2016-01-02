using System;
using System.Collections.Generic;
using CommandLine;
using DbMigrator.Core;

namespace DBMigrator
{
    [Verb("script", HelpText = "Generates a SQL script from the included migrations.")]
    class GenerateScriptVerb : IVerbHandler
    {
        [Option('o', "output-file", Required = true, HelpText = "Filename of the script that will be generated.")]
        public string OutputFilename { get; set; }

        [Option('d', "base-dir", Required = true, HelpText = "Directory containing the migration scripts.")]
        public string BaseDirectory { get; set; }

        [Option('i', "include-dir", Min = 0, Max = 50, HelpText = "Include the migrations from the specified directories.")]
        public IEnumerable<string> IncludeDirectories { get; set; } 

        [Option('p', "params", Min = 0, Max = 100, HelpText = "Parameters that will be replaced in the generated script. Each variable must be specified in the format name=value with no spaces.")]
        public IEnumerable<string> Parameters { get; set; }

        [Option('t', "template-file", Required = false, HelpText = "Filename of the template that will be used to generate the script.")]
        public string TemplateFilename { get; set; }

        [Option('v', "--verbose", DefaultValue = false, HelpText = "Enables verbose logging.")]
        public bool Verbose { get; set; }

        public void Execute()
        {
            var logger = new ConsoleLogger(Verbose);

            var fileSystem = new FileSystem();
            var handler = new ScriptGenerationHandler(fileSystem, logger);
            
            var arguments = ParseParameters(Parameters);

            handler.Execute(OutputFilename, BaseDirectory, IncludeDirectories, arguments, TemplateFilename);
        }

        private Dictionary<string, string> ParseParameters(IEnumerable<string> parameters)
        {
            var result = new Dictionary<string, string>();

            foreach (var param in parameters)
            {
                var parts = param.Split('=');

                result.Add(parts[0], parts[1]);
            }

            return result;
        }
    }
}
