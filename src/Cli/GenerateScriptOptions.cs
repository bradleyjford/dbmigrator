using CommandLine;
using DbMigrator.Core;

namespace DbMigrator.Cli;

[Verb("script", HelpText = "Generates a SQL script from the included migrations.")]
class GenerateScriptVerb : IVerbHandler
{
    public GenerateScriptVerb(
        string outputFilename, 
        string baseDirectory, 
        ICollection<string> includeDirectories, 
        IEnumerable<string> parameters, 
        string templateFilename, 
        bool verbose)
    {
        OutputFilename = outputFilename;
        BaseDirectory = baseDirectory;
        IncludeDirectories = includeDirectories;
        Parameters = parameters;
        TemplateFilename = templateFilename;
        Verbose = verbose;
    }

    [Option('o', "output-file", Required = true, 
        HelpText = "Filename of the script that will be generated.")]
    public string OutputFilename { get; }

    [Option('d', "base-dir", Required = true, 
        HelpText = "Directory containing the migration scripts.")]
    public string BaseDirectory { get; }

    [Option('i', "include-dir", 
        HelpText = "Include the migrations from the specified directories.")]
    public ICollection<string> IncludeDirectories { get; }

    [Option('p', "params", 
        HelpText = "Parameters that will be replaced in the generated script. Each variable must be specified in the format name=value with no spaces.")]
    public IEnumerable<string> Parameters { get; }

    [Option('t', "template-file", Required = false,
        HelpText = "Filename of the template that will be used to generate the script.")]
    public string TemplateFilename { get; }

    [Option('v', "--verbose", Default = false,
        HelpText = "Enables verbose logging.")]
    public bool Verbose { get; }

    public async Task<int> ExecuteAsync()
    {
        var logger = new ConsoleLogger(Verbose);

        var fileSystem = new FileSystem("*.sql");
        var handler = new ScriptGenerationHandler(fileSystem, logger);
            
        var arguments = ParseParameters(Parameters);

        await handler.Execute(
            OutputFilename, 
            BaseDirectory, 
            IncludeDirectories.ToArray(), 
            arguments, 
            TemplateFilename);

        return 0;
    }

    Dictionary<string, string> ParseParameters(IEnumerable<string> parameters)
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