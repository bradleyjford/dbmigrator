using System;
using System.Data.SqlClient;

namespace DbMigrator.SqlClient
{
    internal class RecreateDatabaseCommand
    {
        public RecreateDatabaseCommand(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; private set; }
    }

    internal class RecreateDatabaseCommandHandler : SqlClientCommandHandler
    {
        private const string DropDatabaseCommandFormat = @"
IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'{0}')
BEGIN
    ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    ALTER DATABASE [{0}] SET SINGLE_USER

    DROP DATABASE [{0}]
END";

        private const string CreateDatabaseCommandFormat = @"CREATE DATABASE [{0}]";

        public RecreateDatabaseCommandHandler(ILogger logger) 
            : base(logger)
        {
        }

        public void Handle(RecreateDatabaseCommand command)
        {
            var databaseName = GetDatabaseName(command.ConnectionString);

            var connectionString = GetMasterDatabaseConnectionString(command.ConnectionString);

            using (var sqlConnection = OpenConnection(connectionString))
            {
                sqlConnection.Open();

                DropDatabaseIfExists(sqlConnection, databaseName);
                CreateDatabase(sqlConnection, databaseName);
            }
        }

        private static void DropDatabaseIfExists(SqlConnection sqlConnection, string databaseName)
        {
            using (var sqlCommand = sqlConnection.CreateCommand())
            {
                sqlCommand.CommandText = String.Format(DropDatabaseCommandFormat, databaseName);

                sqlCommand.ExecuteNonQuery();
            }
        }

        private static void CreateDatabase(SqlConnection sqlConnection, string databaseName)
        {
            using (var sqlCommand = sqlConnection.CreateCommand())
            {
                sqlCommand.CommandText = String.Format(CreateDatabaseCommandFormat, databaseName);

                sqlCommand.ExecuteNonQuery();
            }
        }
    }
}
