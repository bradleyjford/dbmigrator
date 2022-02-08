using System;
using System.Collections.Generic;
using System.IO;

namespace DBMigrator.Core
{
    public class ScriptGenerationHandler
    {
        readonly IFileSystem _fileSystem;
        readonly ILogger _logger;

        public ScriptGenerationHandler(IFileSystem fileSystem, ILogger logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;
        }

        public void Execute(
            string outputFilename, 
            string basePath, 
            ICollection<string> includeDirectories, 
            Dictionary<string, string> arguments, 
            string templateFilename)
        {
            LogBeginInfo(outputFilename, basePath, includeDirectories);

            EnsurePathsValid(outputFilename, basePath, includeDirectories);

            var template = GetTemplate(templateFilename);

            using (var stream = _fileSystem.OpenFile(outputFilename))
            using (var streamWriter = new StreamWriter(stream))
            {
                var scriptWriter = new ScriptWriter(streamWriter, template);
                var scriptGenerator = new ScriptGenerator(_fileSystem, _logger);

                scriptGenerator.Generate(scriptWriter, basePath, includeDirectories, arguments);

                stream.SetLength(stream.Position);
            }

            LogEndInfo();
        }

        void LogBeginInfo(string outputFilename, string basePath, IEnumerable<string> includeDirectories)
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

        string GetTemplate(string filename)
        {
            if (String.IsNullOrEmpty(filename))
            {
                return Scripts.ScriptTemplate;
            }

            using (var templateStream = _fileSystem.OpenFileReadOnly(filename))
            using (var reader = new StreamReader(templateStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
