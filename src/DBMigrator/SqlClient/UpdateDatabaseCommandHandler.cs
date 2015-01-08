using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DbMigrator.SqlClient
{
    internal class UpdateDatabaseArguments
    {
        public string ConnectionString { get; private set; }
        public string[] IncludeDirectories { get; set; }
        public Dictionary<string, string> Arguments { get; set; }

        public bool Backup { get; set; }
        public string BackupFilename { get; set; }
    }

    internal class UpdateDatabaseCommandHandler
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        private readonly ScriptFileBatchParser _scriptFileBatchParser;

        public UpdateDatabaseCommandHandler(
            IFileSystem fileSystem,
            ILogger logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;

            _scriptFileBatchParser = new ScriptFileBatchParser(fileSystem);
        }

        public void Handle(UpdateDatabaseArguments args)
        {
            var databaseName = GetDatabaseName(args.ConnectionString);

            using (var connection = OpenConnection(args.ConnectionString))
            {
                try
                {
                    EnterSinlgeUserMode(connection, databaseName);

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            EnsureSchemaMigrationTableExists(connection);

                            ExecuteUpdateScripts(connection, args);

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            _logger.Error(ex.Message);
                        }
                    }
                }
                finally
                {
                    ExitSingleUserMode(connection, databaseName);
                }
            }
        }

        protected SqlConnection OpenConnection(string connectionString)
        {
            var connection = new SqlConnection(connectionString);

            connection.InfoMessage += OnInfoMessageReceived;

            connection.Open();

            return connection;
        }

        private void OnInfoMessageReceived(object sender, SqlInfoMessageEventArgs e)
        {
            foreach (SqlError error in e.Errors)
            {
                _logger.Info("Line {0}: {1}", error.LineNumber, error.Message);
            }
        }

        private void ExecuteUpdateScripts(SqlConnection connection, UpdateDatabaseArguments args)
        {
            var scriptFiles = _fileSystem.GetScriptFileNames(args.IncludeDirectories).OrderBy(s => s, new FilenameComparer());

            foreach (var scriptFile in scriptFiles)
            {
                foreach (var scriptBatch in _scriptFileBatchParser.GetScriptBatches(scriptFile, args.Arguments))
                {
                    connection.ExecuteNonQueryCommand(scriptBatch);
                }
            }
        }

        private void EnterSinlgeUserMode(SqlConnection connection, string databaseName)
        {
            var commandText = String.Format(Scripts.SetDatabaseSingleUser, databaseName);

            connection.ExecuteNonQueryCommand(commandText);
        }

        private void ExitSingleUserMode(SqlConnection connection, string databaseName)
        {
            var commandText = String.Format(Scripts.SetDatabaseMultiUser, databaseName);

            connection.ExecuteNonQueryCommand(commandText);
        }

        private void EnsureSchemaMigrationTableExists(SqlConnection connection)
        {
            connection.ExecuteNonQueryCommand(Scripts.EnsureMigrationTableExists);
        }

        private string GetDatabaseName(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);

            return builder.InitialCatalog;
        }

        private string GetMasterDatabaseConnectionString(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);

            builder.InitialCatalog = "master";

            return builder.ToString();
        }
    }
}
