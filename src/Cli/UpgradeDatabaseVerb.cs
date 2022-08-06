using System.Collections.Immutable;
using CommandLine;
using DbMigrator.Core;
using DbMigrator.Core.SqlClient;

namespace DbMigrator.Cli;

[Verb("upgrade", HelpText = "Applies the included migrations to the specified database.")]
class UpgradeDatabaseVerb : IVerbHandler
{
    public UpgradeDatabaseVerb(
        string baseDirectory, 
        string connectionString, 
        IEnumerable<string> includeDirectories, 
        bool backupDatabase, 
        string backupFilename, 
        bool recreateDatabase, 
        IEnumerable<string> variables, 
        bool verbose)
    {
        BaseDirectory = baseDirectory;
        ConnectionString = connectionString;
        IncludeDirectories = includeDirectories;
        BackupDatabase = backupDatabase;
        BackupFilename = backupFilename;
        RecreateDatabase = recreateDatabase;
        Variables = variables;
        Verbose = verbose;
    }

    [Option('d', "base-dir", Required = true,
        HelpText = "Directory containing the migration scripts that will be applied to the database")]
    public string BaseDirectory { get; }

    [Option('c', "connection-string", Required = true,
        HelpText = "Connection string used to connect to the database.")]
    public string ConnectionString { get; }

    [Option('i', "include-dir",
        HelpText = "Include the migrations from the specified directories.")]
    public IEnumerable<string> IncludeDirectories { get; }

    [Option('b', "backup", Required = false, Default = false,
        HelpText = "Create a database backup prior to applying any migrations.")]
    public bool BackupDatabase { get; }

    [Option('f', "backup-file", Required = false,
        HelpText = "Path and filename of the database backup that will be created.")]
    public string BackupFilename { get; }

    [Option('r', "recreate-database", Required = false, Default = false,
        HelpText = "Drop and recreate the database rather than upgrading.")]
    public bool RecreateDatabase { get; }

    [Option('p', "params", Required = false,
        HelpText = "Parameters that will be replaced in the generated script. Each variable must be specified in the format name=value with no spaces.")]
    public IEnumerable<string> Variables { get; }

    [Option('v', "--verbose", Default = false, HelpText = "Enables verbose logging.")]
    public bool Verbose { get; }

    public async Task<int> ExecuteAsync()
    {
        var logger = new ConsoleLogger(Verbose);

        var fileSystem = new FileSystem("*.sql");
        var handler = new DatabaseUpgradeHandler(fileSystem, new MsSqlScriptFileBatchParser(fileSystem), logger);

        var arguments = ParseParameters(Variables);

        await handler.Execute(
            ConnectionString, 
            BaseDirectory, 
            IncludeDirectories.ToArray(), 
            arguments,
            BackupDatabase, 
            BackupFilename, 
            RecreateDatabase);

        return 0;
    }

    ImmutableDictionary<string, string> ParseParameters(IEnumerable<string> parameters)
    {
        var result = ImmutableDictionary.CreateBuilder<string, string>();

        foreach (var param in parameters)
        {
            var parts = param.Split('=');

            result.Add(parts[0], parts[1]);
        }

        return result.ToImmutableDictionary();
    }
}