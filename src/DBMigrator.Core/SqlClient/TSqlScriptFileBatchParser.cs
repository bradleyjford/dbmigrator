using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DBMigrator.Core
{
    public class TSqlScriptFileBatchParser : IScriptFileBatchParser
    {
        readonly IFileSystem _fileSystem;

        static readonly Regex BatchTerminatorRegex = new Regex(
            @"^[\s]*GO[;\s]*$", 
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline, 
            TimeSpan.FromSeconds(2));

        static readonly Regex ParameterRegex = new Regex(
            @"\$\((?<name>[^)]+)\)", 
            RegexOptions.Compiled | RegexOptions.IgnoreCase,
            TimeSpan.FromSeconds(2));

        const int BufferSize = 20 * 1024;

        public TSqlScriptFileBatchParser(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public IEnumerable<string> GetScriptBatches(string filename, IDictionary<string, string> arguments)
        {
            var buffer = new StringBuilder(BufferSize);

            var lineNumber = 0;
            var batchStartLineNumber = 0;

            using (var scriptFile = _fileSystem.OpenFileReadOnly(filename))
            using (var reader = new StreamReader(scriptFile))
            {
                string scriptBatch;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    lineNumber++;

                    if (BatchTerminatorRegex.IsMatch(line))
                    {
                        scriptBatch = Preprocess(buffer.ToString(), arguments);

                        yield return String.Format($"LINENO {batchStartLineNumber}{Environment.NewLine}EXECUTE('{scriptBatch}')");

                        buffer.Clear();

                        batchStartLineNumber = lineNumber + 1;
                    }
                    else
                    {
                        buffer.AppendLine(line);
                    }
                }

                if (buffer.ToString().Trim().Length > 0)
                {
                    scriptBatch = Preprocess(buffer.ToString(), arguments);

                    yield return String.Format($"LINENO {batchStartLineNumber}{Environment.NewLine}EXECUTE('{scriptBatch}')");
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

            script = script.Replace("'", "''");

            return script.TrimEnd();
        }
    }
}
