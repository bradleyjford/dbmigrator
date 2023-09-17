using Microsoft.Data.SqlClient;

namespace DbMigrator.Core.SqlClient;

sealed class SqlDbMigrationManager : IDbMigrationManager
{
    const string MasterDatabaseName = "master";

    readonly string _connectionString;
    readonly string _databaseName;
    readonly ILogger _logger;

    SqlConnection _connection = default!;

    public static async Task<SqlDbMigrationManager> CreateAsync(string connectionString, string databaseName, ILogger logger)
    {
        var result = new SqlDbMigrationManager(connectionString, databaseName, logger);

        await result.OpenConnectionAsync();
        await result.EnsureDatabaseExistsAsync();
        await result.EnterSingleUserModeAsync();

        return result;
    }
    
    SqlDbMigrationManager(string connectionString, string databaseName, ILogger logger)
    {
        _connectionString = connectionString;
        _databaseName = databaseName;
        _logger = logger;
    }

    public async ValueTask DisposeAsync()
    {
        await ExitSingleUserModeAsync().ConfigureAwait(false);
        await _connection.CloseAsync().ConfigureAwait(false);
    }
    
    public IDbMigrator CreateMigrator()
    {
        return new SqlDbMigrator(_connection!, _logger);
    }
    
    async Task OpenConnectionAsync()
    {
        _connection = new SqlConnection(_connectionString);
        _connection.InfoMessage += OnInfoMessageReceived;

        await _connection.OpenAsync();
    }

    void OnInfoMessageReceived(object sender, SqlInfoMessageEventArgs e)
    {
        foreach (SqlError error in e.Errors)
        {
            _logger.Info(error.Message);
        }
    }

    public async Task BackupDatabaseAsync(string filename)
    {
        var commandText = string.Format(Scripts.BackupDatabase, _databaseName, filename);

        await _connection!.ExecuteNonQueryCommandAsync(commandText);
    }

    public async Task EnsureDatabaseExistsAsync()
    {
        _connection!.ChangeDatabase(MasterDatabaseName);

        var commandText = string.Format(Scripts.CreateDatabase, _databaseName);
        await _connection.ExecuteNonQueryCommandAsync(commandText);

        _connection.ChangeDatabase(_databaseName);
    }
    
    public async Task RecreateDatabaseAsync()
    {
        _connection!.ChangeDatabase(MasterDatabaseName);

        var commandText = string.Format(Scripts.DropDatabase, _databaseName);
        await _connection.ExecuteNonQueryCommandAsync(commandText);

        commandText = string.Format(Scripts.CreateDatabase, _databaseName);
        await _connection.ExecuteNonQueryCommandAsync(commandText);
    }
        
    async Task EnterSingleUserModeAsync()
    {
        var commandText = string.Format(Scripts.SetDatabaseSingleUser, _databaseName);

        await _connection!.ExecuteNonQueryCommandAsync(commandText);
    }

    async Task ExitSingleUserModeAsync()
    {
        var commandText = string.Format(Scripts.SetDatabaseMultiUser, _databaseName);

        await _connection!.ExecuteNonQueryCommandAsync(commandText);
    }

    string GetDatabaseName(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);

        return builder.InitialCatalog;
    }
}