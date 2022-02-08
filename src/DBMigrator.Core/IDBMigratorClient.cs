using System;

namespace DBMigrator.Core
{
    public interface IDBMigratorClient
    {
        void BackupDatabase(string name);
        void DropDatabase(string name);
        void CreateDatabase(string name);
        void ExecuteDbScript(string script);
    }
}
