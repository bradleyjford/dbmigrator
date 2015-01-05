using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DbMigrator
{
    internal static class ScriptFileBatchParser
    {
        private static readonly Regex BatchTerminatorRegex = 
            new Regex(@"^[\s]*GO[\s]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private const int BufferSize = 4096;

        public static IEnumerable<string> GetScriptBatches(IFileSystem fileSystem, string scriptPath)
        {
            var buffer = new StringBuilder(BufferSize);

            using (var scriptFile = fileSystem.OpenFile(scriptPath))
            using (var reader = new StreamReader(scriptFile))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (BatchTerminatorRegex.IsMatch(line))
                    {
                        yield return buffer.ToString();

                        buffer.Clear();
                    }
                    else
                    {
                        buffer.AppendLine(line);
                    }
                }
            }
        }
    }
}
