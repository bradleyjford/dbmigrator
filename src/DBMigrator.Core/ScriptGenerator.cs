using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DbMigrator.Core
{
    internal class ScriptGenerator
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        private readonly ScriptFileBatchParser _scriptBatchParser;

        public ScriptGenerator(IFileSystem fileSystem, ILogger logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;

            _scriptBatchParser = new ScriptFileBatchParser(fileSystem);
        }

        public void Generate(
            ScriptWriter scriptWriter,
            string basePath,
            IEnumerable<string> includeDirectories, 
            IDictionary<string, string> arguments)
        {
            var scriptFilenames = _fileSystem.GetScriptFileNames(basePath, includeDirectories)
                .OrderBy(s => s, new FilenameComparer());

            scriptWriter.WriteHeader();

            foreach (var filename in scriptFilenames)
            {
                var batchCount = ProcessScriptFile(scriptWriter, filename, arguments);

                _logger.Verbose("Merged {0} batch(es) from file \"{1}\"", batchCount, filename);
            }

            scriptWriter.WriteFooter();
        }

        private int ProcessScriptFile(ScriptWriter scriptWriter, string filename, IDictionary<string, string> arguments)
        {
            var batchNumber = 1;
            var scriptBatches = _scriptBatchParser.GetScriptBatches(filename, arguments);

            filename = Path.GetFileNameWithoutExtension(filename);

            foreach (var scriptBatch in scriptBatches)
            {
                scriptWriter.WriteScript(filename, batchNumber++, scriptBatch);
            }

            return batchNumber;
        }
    }
}
