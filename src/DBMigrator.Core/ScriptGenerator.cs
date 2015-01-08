using System;
using System.Collections.Generic;
using System.Linq;

namespace DbMigrator.Core
{
    internal class ScriptGenerator
    {
        private readonly IFileSystem _fileSystem;
        private readonly ScriptFileBatchParser _scriptBatchParser;

        public ScriptGenerator(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _scriptBatchParser = new ScriptFileBatchParser(fileSystem);
        }

        public void Generate(
            ScriptWriter scriptWriter, 
            IEnumerable<string> environments, 
            IDictionary<string, string> arguments)
        {
            var scriptFilenames = _fileSystem.GetScriptFileNames(environments)
                .OrderBy(s => s, new FilenameComparer());

            scriptWriter.WriteHeader();

            foreach (var filename in scriptFilenames)
            {
                ProcessScriptFile(scriptWriter, filename, arguments);
            }

            scriptWriter.WriteFooter();
        }

        private void ProcessScriptFile(ScriptWriter scriptWriter, string filename, IDictionary<string, string> arguments)
        {
            var batchNumber = 1;
            var scriptBatches = _scriptBatchParser.GetScriptBatches(filename, arguments);

            foreach (var scriptBatch in scriptBatches)
            {
                scriptWriter.WriteScript(filename, batchNumber++, scriptBatch);
            }
        }
    }
}
