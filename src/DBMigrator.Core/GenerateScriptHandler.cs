using System;
using System.Collections.Generic;
using System.IO;

namespace DbMigrator.Core
{
    public class GenerateScriptHandler
    {
        private readonly IFileSystem _fileSystem;

        public GenerateScriptHandler(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void Execute(
            string outputFilename, 
            IEnumerable<string> includeDirectories,
            Dictionary<string, string> arguments,
            string templateFilename)
        {
            var template = GetTemmplate(templateFilename);

            using (var stream = _fileSystem.OpenFile(outputFilename))
            using (var streamWriter = new StreamWriter(stream))
            {
                var scriptWriter = new ScriptWriter(streamWriter, template);
                var scriptGenerator = new ScriptGenerator(_fileSystem);

                scriptGenerator.Generate(scriptWriter, includeDirectories, arguments);
            }
        }

        private string GetTemmplate(string filename)
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
