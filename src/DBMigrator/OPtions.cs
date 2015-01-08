using System;
using CommandLine;

namespace DBMigrator
{
    class Options
    {
        [VerbOption("script", HelpText = "Generates a SQL script from the included migrations.")]
        public GenerateScriptVerb ScriptVerb { get; set; }
  
        [VerbOption("upgrade", HelpText = "Applies the included migrations to the specified database.")]
        public UpgradeDatabaseVerb UpgradeVerb { get; set; }
    }
}
