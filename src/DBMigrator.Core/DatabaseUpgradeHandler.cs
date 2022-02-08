using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace DBMigrator.Core
{
    public class DatabaseUpgradeHandler
    {
        readonly IFileSystem _fileSystem;
        readonly ILogger _logger;
        readonly IScriptFileBatchParser _scriptFileBatchParser;
        readonly string _batchTemplate;

        public DatabaseUpgradeHandler(
            IFileSystem fileSystem,
            IScriptFileBatchParser scriptBatchParser,
            ILogger logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;

            _scriptFileBatchParser = scriptBatchParser;

            _batchTemplate = Scripts.ScriptBatchTemplate;
        }

        public void Execute(
            string connectionString, 
            string basePath,
            IEnumerable<string> includeDirectories, 
            Dictionary<string, string> arguments, 
            bool backup, 
            string backupFilename, 
            bool recreateDatabase )
        {
            var databaseName = GetDatabaseName(connectionString);

            using (var connection = OpenConnection(connectionString))
            {
                EnterSingleUserMode(connection, databaseName);

                try
                {
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
                            EnsureSchemaMigrationTableExists(connection, transaction);

                            ExecuteUpdateScripts(connection, transaction, basePath, includeDirectories, arguments);

                            transaction.Commit();
                        }
                        catch (SqlException ex)
                        {
                            transaction.Rollback();

                            _logger.Error($"Line {ex.LineNumber}: {ex.Message}");
                        }
                    }
                }
                finally
                {
                    ExitSingleUserMode(connection, databaseName);
                }
            }
        }

        SqlConnection OpenConnection(string connectionString)
        {
            var connection = new SqlConnection(connectionString);

            connection.InfoMessage += OnInfoMessageReceived;

            connection.Open();

            return connection;
        }

        void OnInfoMessageReceived(object sender, SqlInfoMessageEventArgs e)
        {
            foreach (SqlError error in e.Errors)
            {
                _logger.Info(error.Message);
            }
        }

        void CreateBackup(SqlConnection connection, string databaseName, string filename)
        {
            var commandText = String.Format(Scripts.BackupDatabase, databaseName, filename);

            connection.ExecuteNonQueryCommand(commandText);
        }

        void RecreateDatabase(SqlConnection connection, string databaseName)
        {
            connection.ChangeDatabase("master");

            var commandText = String.Format(Scripts.DropDatabase, databaseName);

            connection.ExecuteNonQueryCommand(commandText);

            commandText = String.Format(Scripts.CreateDatabase, databaseName);

            connection.ExecuteNonQueryCommand(commandText);

            connection.ChangeDatabase(databaseName);
        }

        void ExecuteUpdateScripts(
            SqlConnection connection,
            SqlTransaction transaction,
            string basePath,
            IEnumerable<string> includeDirectories,
            Dictionary<string, string> arguments)
        {
            var scriptFiles = _fileSystem.GetScriptFileNames(basePath, includeDirectories)
                .OrderBy(s => s, new FilenameComparer());

            foreach (var scriptFile in scriptFiles)
            {
                var batch = 1;

                var normalizedFilename = Path.GetFileNameWithoutExtension(scriptFile);

                foreach (var scriptBatch in _scriptFileBatchParser.GetScriptBatches(scriptFile, arguments))
                {
                    var commandText = PrepareBatch(normalizedFilename, batch, scriptBatch);

                    connection.ExecuteNonQueryCommand(commandText, transaction: transaction);

                    batch++;
                }
            }
        }

        string PrepareBatch(string filename, int batch, string script)
        {
            var result = _batchTemplate;

            result = result.Replace(TemplateToken.Filename, filename);
            result = result.Replace(TemplateToken.Batch, batch.ToString());
            result = result.Replace(TemplateToken.Script, script);

            return result;
        }

        void EnterSingleUserMode(SqlConnection connection, string databaseName)
        {
            var commandText = String.Format(Scripts.SetDatabaseSingleUser, databaseName);

            connection.ExecuteNonQueryCommand(commandText);
        }

        void ExitSingleUserMode(SqlConnection connection, string databaseName)
        {
            var commandText = String.Format(Scripts.SetDatabaseMultiUser, databaseName);

            connection.ExecuteNonQueryCommand(commandText);
        }

        void EnsureSchemaMigrationTableExists(SqlConnection connection, SqlTransaction transaction)
        {
            connection.ExecuteNonQueryCommand(Scripts.EnsureMigrationTableExists, transaction: transaction);
        }

        string GetDatabaseName(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);

            return builder.InitialCatalog;
        }
    }
}
