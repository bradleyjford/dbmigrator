using System;
using System.Data.SqlClient;

namespace DbMigrator.SqlClient
{
    internal class BackupCommand
    {
        public BackupCommand(string connectionString, string backupPath)
        {
            ConnectionString = connectionString;
            BackupPath = backupPath;
        }

        public string ConnectionString { get; private set; }
        public string BackupPath { get; private set; }
    }

    internal class BackupCommandHandler : SqlClientCommandHandler
    {
        private const string BackupCommandFormat = @"BACKUP DATABASE [{0}] TO DISK = '{1}'";

        private readonly ILogger _logger;

        public BackupCommandHandler(ILogger logger) 
            : base(logger)
        {
            _logger = logger;
        }

        public void Handle(BackupCommand command)
        {
            var databaseName = GetDatabaseName(command.ConnectionString);

            var commandText = String.Format(BackupCommandFormat, databaseName, command.BackupPath);

            using (var sqlConnection = OpenConnection(command.ConnectionString))
            using (var sqlCommand = new SqlCommand(commandText, sqlConnection))
            {
                sqlCommand.CommandTimeout = 0;

                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
            }
        }
    }
}
