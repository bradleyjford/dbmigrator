using System;
using System.Collections.Generic;

namespace DBMigrator.Core
{
    public interface IScriptFileBatchParser
    {
        IEnumerable<string> GetScriptBatches(string filename, IDictionary<string, string> arguments);
    }
}
