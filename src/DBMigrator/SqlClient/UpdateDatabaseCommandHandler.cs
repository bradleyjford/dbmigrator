using System;

namespace DbMigrator.SqlClient
{
    internal class UpdateDatabaseCommand
    {
        public UpdateDatabaseCommand(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; private set; }
    }

    internal class UpdateDatabaseCommandHandler : SqlClientCommandHandler
    {
        public UpdateDatabaseCommandHandler(ILogger logger) 
            : base(logger)
        {
        }

        public void Handle()
        {
            
        }
    }
}
