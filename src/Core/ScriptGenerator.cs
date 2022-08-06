using DbMigrator.Core.SqlClient;

namespace DbMigrator.Core;

sealed class ScriptGenerator
{
    readonly IFileSystem _fileSystem;
    readonly ILogger _logger;
    readonly MsSqlScriptFileBatchParser _scriptBatchParser;

    public ScriptGenerator(IFileSystem fileSystem, ILogger logger)
    {
        _fileSystem = fileSystem;
        _logger = logger;

        _scriptBatchParser = new MsSqlScriptFileBatchParser(fileSystem);
    }

    public async Task Generate(
        ScriptWriter scriptWriter,
        string basePath,
        string[] includeDirectories, 
        IDictionary<string, string> arguments)
    {
        await scriptWriter.WriteHeader();

        var scriptFilenames = _fileSystem.GetMigrationScripts(basePath, includeDirectories);

        foreach (var filename in scriptFilenames)
        {
            var batchCount = await ProcessScriptFile(scriptWriter, filename, arguments);

            _logger.Verbose($"Merged {batchCount} batch(es) from file \"{filename}\"");
        }

        await scriptWriter.WriteFooter();
    }

    async Task<int> ProcessScriptFile(ScriptWriter scriptWriter, string filename, IDictionary<string, string> arguments)
    {
        var batchCount = 0;
        var scriptBatches = _scriptBatchParser.GetScriptBatches(filename, arguments);
            
        await foreach (var scriptBatch in scriptBatches)
        {
            batchCount++;
            await scriptWriter.WriteScript(scriptBatch);
        }

        return batchCount;
    }
}