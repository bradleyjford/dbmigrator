using System;
using System.Collections.Generic;
using System.Linq;

namespace DbMigrator
{
    internal class ScriptGenerator
    {
        private readonly IFileSystem _fileSystem;
        private readonly IEnumerable<string> _includeEnvironments;
        private readonly IDictionary<string, string> _arguments;

        public ScriptGenerator(
            IFileSystem fileSystem, 
            IEnumerable<string> includeEnvironments,
            IDictionary<string, string> arguments)
        {
            _fileSystem = fileSystem;
            _includeEnvironments = includeEnvironments;
            _arguments = arguments;
        }

        public void Run(ScriptWriter scriptWriter)
        {
            var scriptPaths = _fileSystem.GetScriptFileNames(_includeEnvironments)
                .OrderBy(s => s, new FileNameComparer());

            scriptWriter.WriteHeader();

            foreach (var scriptPath in scriptPaths)
            {
                ProcessScript(scriptPath, scriptWriter);
            }

            scriptWriter.WriteFooter();
        }

        private void ProcessScript(string scriptPath, ScriptWriter scriptWriter)
        {
            var batchNumber = 1;
            var scriptBatches = ScriptFileBatchParser.GetScriptBatches(_fileSystem, scriptPath);

            foreach (var scriptBatch in scriptBatches)
            {
                var batch = ScriptPreprocessor.Process(scriptBatch, _arguments);

                scriptWriter.WriteScript(scriptPath, batchNumber++, batch);
            }
        }
    }
}
