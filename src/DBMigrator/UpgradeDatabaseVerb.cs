using System;
using CommandLine;
using DbMigrator.Core;

namespace DBMigrator
{
    class UpgradeDatabaseVerb : CommandLineVerb
    {
        [Option('d', "base-dir", Required = true, HelpText = "Directory containing the migration scripts that will be applied to the database")]
        public string BaseDirectory { get; set; }

        [Option('c', "connection-string", Required = true)]
        public string ConnectionString { get; set; }

        [OptionArray('i', "include-dir", Required = false, DefaultValue = new string[0])]
        public string[] IncludeDirectories { get; set; }

        [Option('u', "backup", Required = false, DefaultValue = false)]
        public bool BackupDatabase { get; set; }

        [Option('f', "backup-file", Required = false)]
        public string BackupFilename { get; set; }

        [Option('r', "recreate-database", Required = false, DefaultValue = false, HelpText = "")]
        public bool RecreateDatabase { get; set; }

        [OptionArray('v', "var", Required = false, HelpText = "Variables that will be replaced in the generated script. Each variable must be specified in the format name=value with no spaces.")]
        public string[] Variables { get; set; }

        public override void Execute()
        {
            var logger = new ConsoleLogger();

            var fileSystem = new FileSystem(BaseDirectory);
            var handler = new DatabaseUpgradeHandler(fileSystem, logger);

            var arguments = ParseArguments(Variables);

            handler.Execute(ConnectionString, IncludeDirectories, arguments, BackupDatabase, BackupFilename, RecreateDatabase);
        }
    }
}