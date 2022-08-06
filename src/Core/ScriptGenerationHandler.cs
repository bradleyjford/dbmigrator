using System.IO;
using DbMigrator.Core.SqlClient;

namespace DbMigrator.Core;

public sealed class ScriptGenerationHandler
{
    readonly IFileSystem _fileSystem;
    readonly ILogger _logger;

    public ScriptGenerationHandler(IFileSystem fileSystem, ILogger logger)
    {
        _fileSystem = fileSystem;
        _logger = logger;
    }

    public async Task Execute(
        string outputFilename, 
        string basePath, 
        string[] includeDirectories, 
        Dictionary<string, string> arguments, 
        string templateFilename)
    {
        LogBeginInfo(outputFilename, basePath, includeDirectories);

        EnsurePathsValid(outputFilename, basePath, includeDirectories);

        var template = await GetTemplate(templateFilename);

        using (var stream = _fileSystem.OpenFile(outputFilename))
        using (var streamWriter = new StreamWriter(stream))
        {
            var scriptWriter = new ScriptWriter(streamWriter, template);
            var scriptGenerator = new ScriptGenerator(_fileSystem, _logger);

            await scriptGenerator.Generate(scriptWriter, basePath, includeDirectories, arguments);

            await stream.FlushAsync();
            stream.SetLength(stream.Position);
        }

        LogEndInfo();
    }

    void LogBeginInfo(string outputFilename, string basePath, string[] includeDirectories)
    {
        _logger.Info($"Generating script \"{outputFilename}\" from migrations contained in the following directories:");
        _logger.Info($"    {basePath}");

        foreach (var includedDirectory in includeDirectories)
        {
            _logger.Info($"    {includedDirectory}");
        }

        _logger.Info("");
    }

    void LogEndInfo()
    {
        _logger.Verbose("");
        _logger.Info("Done.");
    }

    void EnsurePathsValid(string outputFilename, string basePath, IEnumerable<string> includeDirectories)
    {
        // Normalise paths and compare 

        // Throw exception if outputFilename is within the basePath or any of the included directories.
    }

    async Task<string> GetTemplate(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            return Scripts.ScriptTemplate;
        }

        await using var templateStream = _fileSystem.OpenFileReadOnly(filename);
        using var reader = new StreamReader(templateStream);
        
        return await reader.ReadToEndAsync();
    }
}