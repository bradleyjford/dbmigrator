using DbMigrator.Core.SqlClient;

namespace DbMigrator.Core;

public class DatabaseUpgradeHandler
{
    readonly IFileSystem _fileSystem;
    readonly ILogger _logger;
    readonly IScriptFileBatchParser _scriptFileBatchParser;

    public DatabaseUpgradeHandler(
        IFileSystem fileSystem,
        IScriptFileBatchParser scriptBatchParser,
        ILogger logger)
    {
        _fileSystem = fileSystem;
        _logger = logger;

        _scriptFileBatchParser = scriptBatchParser;
    }

    public async Task Execute(
        string connectionString, 
        string basePath,
        string[] includeDirectories, 
        IDictionary<string, string> arguments, 
        bool backup, 
        string backupFilename, 
        bool recreateDatabase)
    { 
        await using var migrator = await SqlDbMigrationManager.CreateAsync(connectionString, _logger);
        
        if (backup)
        {
            await migrator.BackupDatabaseAsync(backupFilename);
        }

        if (recreateDatabase)
        {
            await migrator.RecreateDatabaseAsync();
        }

        using var runner = migrator.CreateMigrator();
        
        try
        {
            await runner.EnsureSchemaMigrationTableExistsAsync();
            await ExecuteUpdateScriptsAsync(runner, basePath, includeDirectories, arguments);

            await runner.CommitAsync();
        }
        catch (Exception)
        {
            await runner.RollbackAsync();
        }
    }
        
    async Task ExecuteUpdateScriptsAsync(
        IDbMigrator migrator,
        string basePath,
        string[] includeDirectories,
        IDictionary<string, string> arguments)
    {
        var scriptFiles = _fileSystem.GetMigrationScripts(basePath, includeDirectories);

        foreach (var scriptFile in scriptFiles)
        {
            await ExecuteMigrationScriptAsync(migrator, scriptFile, arguments);
        }
    }
        
    async Task ExecuteMigrationScriptAsync(
        IDbMigrator migrator,
        string scriptFilename,
        IDictionary<string, string> arguments)
    {
        await foreach (var scriptBatch in _scriptFileBatchParser.GetScriptBatches(scriptFilename, arguments))
        {
            await migrator.ApplyMigrationAsync(scriptFilename, scriptBatch); 
        }
    }
}