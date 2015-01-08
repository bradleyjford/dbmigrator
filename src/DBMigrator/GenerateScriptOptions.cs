using System;
using CommandLine;
using DbMigrator.Core;

namespace DBMigrator
{
    class GenerateScriptVerb : CommandLineVerb
    {
        [Option('o', "output-file", Required = true, HelpText = "Filename of the script that will be generated")]
        public string OutputFilename { get; set; }

        [Option('d', "base-dir", Required = true, HelpText = "Directory containing the migration scripts that will be included in the generated script")]
        public string BaseDirectory { get; set; }

        [OptionArray('i', "include-dir", Required = false)]
        public string[] IncludeDirectories { get; set; } 

        [OptionArray('v', "var", Required = false, HelpText = "Variables that will be replaced in the generated script. Each variable must be specified in the format name=value with no spaces.")]
        public string[] Variables { get; set; }

        [Option('t', "template-file", Required = false, HelpText = "Filename of the template that will be used to generate the script")]
        public string TemplateFilename { get; set; }

        public override void Execute()
        {
            var fileSystem = new FileSystem(BaseDirectory);
            var handler = new GenerateScriptHandler(fileSystem);

            var arguments = ParseArguments(Variables);

            handler.Execute(OutputFilename, IncludeDirectories, arguments, TemplateFilename);
        }
    }
}
