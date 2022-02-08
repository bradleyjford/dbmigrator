using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DBMigrator.Core
{
    class ScriptGenerator
    {
        readonly IFileSystem _fileSystem;
        readonly ILogger _logger;
        readonly TSqlScriptFileBatchParser _scriptBatchParser;

        public ScriptGenerator(IFileSystem fileSystem, ILogger logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;

            _scriptBatchParser = new TSqlScriptFileBatchParser(fileSystem);
        }

        public void Generate(
            ScriptWriter scriptWriter,
            string basePath,
            ICollection<string> includeDirectories, 
            IDictionary<string, string> arguments)
        {
            var scriptFilenames = _fileSystem.GetScriptFileNames(basePath, includeDirectories)
                .OrderBy(s => s, new FilenameComparer());

            scriptWriter.WriteHeader();

            foreach (var filename in scriptFilenames)
            {
                var batchCount = ProcessScriptFile(scriptWriter, filename, arguments);

                _logger.Verbose($"Merged {batchCount} batch(es) from file \"{filename}\"");
            }

            scriptWriter.WriteFooter();
        }

        int ProcessScriptFile(ScriptWriter scriptWriter, string filename, IDictionary<string, string> arguments)
        {
            var batchNumber = 1;
            var scriptBatches = _scriptBatchParser.GetScriptBatches(filename, arguments);

            filename = Path.GetFileNameWithoutExtension(filename);

            foreach (var scriptBatch in scriptBatches)
            {
                scriptWriter.WriteScript(filename, batchNumber++, scriptBatch);
            }

            return batchNumber - 1;
        }
    }
}
