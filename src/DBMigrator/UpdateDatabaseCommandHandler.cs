using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DbMigrator
{
    internal class UpdateDatabaseArguments
    {
        public string ConnectionString { get; set; }
        public string[] IncludeDirectories { get; set; }
        public Dictionary<string, string> Arguments { get; set; }

        public bool Backup { get; set; }
        public string BackupFilename { get; set; }

        public bool RecreateDatabase { get; set; }
    }

    internal class UpdateDatabaseCommandHandler
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        private readonly ScriptFileBatchParser _scriptFileBatchParser;
        private readonly string _batchTemplate;

        public UpdateDatabaseCommandHandler(
            IFileSystem fileSystem,
            ILogger logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;

            _scriptFileBatchParser = new ScriptFileBatchParser(fileSystem);

            _batchTemplate = Scripts.ScriptBatchTemplate;
        }

        public void Execute(UpdateDatabaseArguments args)
        {
            var databaseName = GetDatabaseName(args.ConnectionString);

            using (var connection = OpenConnection(args.ConnectionString))
            {
                try
                {
                    EnterSinlgeUserMode(connection, databaseName);

                    if (args.Backup)
                    {
                        CreateBackup(connection, databaseName, args.BackupFilename);
                    }

                    if (args.RecreateDatabase)
                    {
                        RecreateDatabase(connection, databaseName);
                    }

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

        private void ExecuteUpdateScripts(SqlConnection connection, UpdateDatabaseArguments args)
        {
            var scriptFiles = _fileSystem.GetScriptFileNames(args.IncludeDirectories)
                .OrderBy(s => s, new FilenameComparer());

            foreach (var scriptFile in scriptFiles)
            {
                var batch = 1;

                foreach (var scriptBatch in _scriptFileBatchParser.GetScriptBatches(scriptFile, args.Arguments))
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
