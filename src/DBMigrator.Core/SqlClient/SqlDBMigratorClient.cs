using System;
using System.Collections.Generic;
using System.Text;

namespace DBMigrator.Core.SqlClient
{
    class SqlDBMigratorClient : IDBMigratorClient
    {
        public void BackupDatabase(string name)
        {
            throw new NotImplementedException();
        }

        public void DropDatabase(string name)
        {
            throw new NotImplementedException();
        }

        public void CreateDatabase(string name)
        {
            throw new NotImplementedException();
        }

        public void ExecuteDbScript(string script)
        {
            throw new NotImplementedException();
        }
    }
}
