namespace DbMigrator.Core;

public interface IScriptFileBatchParser
{
    IAsyncEnumerable<string> GetScriptBatches(string filename, IDictionary<string, string> arguments);
}