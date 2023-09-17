using DbMigrator.Core;
using DbMigrator.Core.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DbMigrator.Tests;

public class DatabaseMigratorTests
{
    readonly IConfigurationRoot _config;

    public DatabaseMigratorTests()
    {
        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
    }
    
    [Fact]
    public async Task Test()
    {
        var fileSystem = new EmbeddedResourceFileSystem(typeof(DatabaseMigratorTests).Assembly);
        var migrator = new DatabaseUpgradeHandler(
            fileSystem, 
            new MsSqlScriptFileBatchParser(fileSystem),
            new ConsoleLogger(true));

        var args = new Dictionary<string, string>();

        await migrator.Execute(
            _config.GetConnectionString("Default"),
            "Test01",
            "scripts/base",
            new [] { "scripts/dev" }, 
            args,
            false,
            "",
            true);
    }
}