using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DbMigrator.Core
{
    internal class ScriptFileBatchParser
    {
        private readonly IFileSystem _fileSystem;

        private static readonly Regex BatchTerminatorRegex = 
            new Regex(@"^[\s]*GO[\s]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private static readonly Regex ParameterRegex =
            new Regex(@"\$\((?<name>[^)]+)\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private const int BufferSize = 20 * 1024;

        public ScriptFileBatchParser(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public IEnumerable<string> GetScriptBatches(string filename, IDictionary<string, string> arguments)
        {
            var buffer = new StringBuilder(BufferSize);

            var lineNumber = 0;
            var batchStartLineNumber = 0;

            using (var scriptFile = _fileSystem.OpenFile(filename))
            using (var reader = new StreamReader(scriptFile))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    lineNumber++;

                    if (BatchTerminatorRegex.IsMatch(line))
                    {
                        var scriptBatch = Preprocess(buffer.ToString(), arguments);

                        yield return String.Format("LineNo {0} {1}", batchStartLineNumber, scriptBatch);

                        buffer.Clear();

                        batchStartLineNumber = lineNumber + 1;
                    }
                    else
                    {
                        buffer.AppendLine(line);
                    }
                }
            }
        }

        private string Preprocess(string script, IDictionary<string, string> arguments)
        {
            foreach (var argument in arguments)
            {
                script = script.Replace("$(" + argument.Key + ")", argument.Value);
            }

            if (ParameterRegex.IsMatch(script))
            {
                // TODO: fix exception message
                throw new Exception("Unresolved script parameter ...");
            }

            return script;
        }
    }
}
