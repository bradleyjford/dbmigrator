using System;
using System.Collections.Generic;
using CommandLine;
using DbMigrator.Core;

namespace DBMigrator
{
    [Verb("upgrade", HelpText = "Applies the included migrations to the specified database.")]
    class UpgradeDatabaseVerb : IVerbHandler
    {
        [Option('d', "base-dir", Required = true, HelpText = "Directory containing the migration scripts that will be applied to the database")]
        public string BaseDirectory { get; set; }

        [Option('c', "connection-string", Required = true, HelpText = "Connection string used to connect to the database.")]
        public string ConnectionString { get; set; }

        [Option('i', "include-dir", Min = 0, Max = 50, HelpText = "Include the migrations from the specified directories.")]
        public IEnumerable<string> IncludeDirectories { get; set; }

        [Option('b', "backup", Required = false, DefaultValue = false, HelpText = "Create a database backup prior to applying any migrations.")]
        public bool BackupDatabase { get; set; }

        [Option('f', "backup-file", Required = false, HelpText = "Path and filename of the database backup that will be created.")]
        public string BackupFilename { get; set; }

        [Option('r', "recreate-database", Required = false, DefaultValue = false, HelpText = "Drop and recreate the database rather than upgrading.")]
        public bool RecreateDatabase { get; set; }

        [Option('p', "params", Required = false, HelpText = "Parameters that will be replaced in the generated script. Each variable must be specified in the format name=value with no spaces.")]
        public IEnumerable<string> Variables { get; set; }

        [Option('v', "--verbose", DefaultValue = false, HelpText = "Enables verbose logging.")]
        public bool Verbose { get; set; }

        public void Execute()
        {
            var logger = new ConsoleLogger(Verbose);

            var fileSystem = new FileSystem();
            var handler = new DatabaseUpgradeHandler(fileSystem, logger);

            var arguments = ParseParameters(Variables);

            handler.Execute(
                ConnectionString, 
                BaseDirectory, 
                IncludeDirectories, 
                arguments, BackupDatabase, 
                BackupFilename, 
                RecreateDatabase);
        }

        protected Dictionary<string, string> ParseParameters(IEnumerable<string> parameters)
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
