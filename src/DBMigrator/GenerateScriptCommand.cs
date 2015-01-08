using System;
using System.Collections.Generic;
using System.IO;

namespace DbMigrator
{
    public class GenerateScriptCommand
    {
        public string OutputFilename { get; set; }
        public string[] IncludeDirectories { get; set; }
        public Dictionary<string, string> Arguments { get; set; }
        public string TemplateFilename { get; set; }
    }

    internal class GenerateScriptCommandHandler
    {
        private readonly IFileSystem _fileSystem;

        public GenerateScriptCommandHandler(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void Execute(GenerateScriptCommand command)
        {
            var template = GetTemmplate(command);

            using (var stream = _fileSystem.OpenFile(command.OutputFilename))
            using (var streamWriter = new StreamWriter(stream))
            {
                var scriptWriter = new ScriptWriter(streamWriter, template);
                var scriptGenerator = new ScriptGenerator(_fileSystem);

                scriptGenerator.Run(scriptWriter, command.IncludeDirectories, command.Arguments);
            }
        }

        private string GetTemmplate(GenerateScriptCommand command)
        {
            if (String.IsNullOrEmpty(command.TemplateFilename))
            {
                return Scripts.ScriptTemplate;
            }

            using (var templateStream = _fileSystem.OpenFileReadOnly(command.TemplateFilename))
            using (var reader = new StreamReader(templateStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
