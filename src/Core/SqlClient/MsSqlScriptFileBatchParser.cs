using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DbMigrator.Core.SqlClient;

public sealed class MsSqlScriptFileBatchParser : IScriptFileBatchParser
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

    public MsSqlScriptFileBatchParser(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public async IAsyncEnumerable<string> GetScriptBatches(string filename, IDictionary<string, string> arguments)
    {
        var buffer = new StringBuilder(BufferSize);

        var batchNumber = 1;
        var lineNumber = 0;
        var batchStartLineNumber = 0;

        await using var scriptFile = _fileSystem.OpenFileReadOnly(filename);
        using var reader = new StreamReader(scriptFile);
        
        string scriptBatch;
                
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            lineNumber++;

            if (BatchTerminatorRegex.IsMatch(line!))
            {
                scriptBatch = buffer.ToString().Trim();

                if (scriptBatch.Length > 0)
                {
                    scriptBatch = ApplyBatchTemplate(filename, batchNumber, scriptBatch, arguments);

                    yield return string.Format($"LINENO {batchStartLineNumber}{Environment.NewLine}EXECUTE('{scriptBatch}')");
                }

                buffer.Clear();
                        
                batchNumber++;
                batchStartLineNumber = lineNumber + 1;
            }
            else
            {
                buffer.AppendLine(line);
            }
        }

        scriptBatch = buffer.ToString().Trim();
                
        if (scriptBatch.Length > 0)
        {
            scriptBatch = ApplyBatchTemplate(filename, batchNumber, scriptBatch, arguments);

            yield return string.Format($"LINENO {batchStartLineNumber}{Environment.NewLine}EXECUTE('{scriptBatch}')");
        }
    }

    string ApplyBatchTemplate(string filename, int batch, string script, IDictionary<string, string> arguments)
    {
        script = SubstituteArguments(script, arguments);

        var result = Scripts.ScriptBatchTemplate;

        result = result.Replace(TemplateToken.Filename, filename);
        result = result.Replace(TemplateToken.Batch, batch.ToString());
        result = result.Replace(TemplateToken.Script, script);

        return result;
    }
    
    string SubstituteArguments(string script, IDictionary<string, string> arguments)
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

        return script.Replace("'", "''");
    }
}