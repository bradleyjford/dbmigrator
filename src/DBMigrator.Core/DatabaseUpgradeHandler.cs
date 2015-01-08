using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DbMigrator.Core
{
    public class DatabaseUpgradeHandler
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        private readonly ScriptFileBatchParser _scriptFileBatchParser;
        private readonly string _batchTemplate;

        public DatabaseUpgradeHandler(
            IFileSystem fileSystem,
            ILogger logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;

            _scriptFileBatchParser = new ScriptFileBatchParser(fileSystem);

            _batchTemplate = Scripts.ScriptBatchTemplate;
        }

        public void Execute(
            string connectionString, 
            IEnumerable<string> includeDirectories, 
            Dictionary<string, string> arguments, 
            bool backup, 
            string backupFilename, 
            bool recreateDatabase )
        {
            var databaseName = GetDatabaseName(connectionString);

            using (var connection = OpenConnection(connectionString))
            {
                try
                {
                    EnterSinlgeUserMode(connection, databaseName);

                    if (backup)
                    {
                        CreateBackup(connection, databaseName, backupFilename);
                    }

                    if (recreateDatabase)
                    {
                        RecreateDatabase(connection, databaseName);
                    }

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            EnsureSchemaMigrationTableExists(connection);

                            ExecuteUpdateScripts(connection, includeDirectories, arguments);

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

        private SqlConnection OpenConnection(string connectionString)
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

        private void CreateBackup(SqlConnection connection, string databaseName, string filename)
        {
            var commandText = String.Format(Scripts.BackupDatabase, databaseName, filename);

            connection.ExecuteNonQueryCommand(commandText);
        }

        private void RecreateDatabase(SqlConnection connection, string databaseName)
        {
            connection.ChangeDatabase("master");

            var commandText = String.Format(Scripts.DropDatabase, databaseName);

            connection.ExecuteNonQueryCommand(commandText);

            commandText = String.Format(Scripts.CreateDatabase, databaseName);

            connection.ExecuteNonQueryCommand(commandText);

            connection.ChangeDatabase(databaseName);
        }

        private void ExecuteUpdateScripts(
            SqlConnection connection,
            IEnumerable<string> includeDirectories,
            Dictionary<string, string> arguments)
        {
            var scriptFiles = _fileSystem.GetScriptFileNames(includeDirectories)
                .OrderBy(s => s, new FilenameComparer());

            foreach (var scriptFile in scriptFiles)
            {
                var batch = 1;

                foreach (var scriptBatch in _scriptFileBatchParser.GetScriptBatches(scriptFile, arguments))
                {
                    var commandText = PrepareBatch(scriptFile, batch, scriptBatch);

                    connection.ExecuteNonQueryCommand(commandText);

                    batch++;
                }
            }
        }

        private string PrepareBatch(string filename, int batch, string script)
        {
            var result = _batchTemplate;

            result = result.Replace(TemplateToken.Filename, filename);
            result = result.Replace(TemplateToken.Batch, batch.ToString());
            result = result.Replace(TemplateToken.Script, script);

            return result;
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
    }
}
