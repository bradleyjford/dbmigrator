using System;
using System.Data.SqlClient;

namespace DbMigrator.SqlClient
{
    internal abstract class SqlClientCommandHandler
    {
        private readonly ILogger _logger;

        protected SqlClientCommandHandler(ILogger logger)
        {
            _logger = logger;
        }

        protected SqlConnection OpenConnection(string connectionString)
        {
            var connection = new SqlConnection(connectionString);

            connection.FireInfoMessageEventOnUserErrors = true;
            connection.InfoMessage += Connection_InfoMessage;

            connection.Open();

            return connection;
        }

        private void Connection_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            foreach (SqlError error in e.Errors)
            {
                _logger.Error("");
            }
        }

        protected string GetDatabaseName(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);

            return builder.InitialCatalog;
        }

        protected string GetMasterDatabaseConnectionString(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);

            builder.InitialCatalog = "master";

            return builder.ToString();
        }
    }
}
