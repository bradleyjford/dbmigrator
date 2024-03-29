using Microsoft.Data.SqlClient;

namespace DbMigrator.Core.SqlClient;

public sealed class SqlDbMigrator : IDbMigrator
{
    readonly SqlConnection _connection;
    readonly SqlTransaction _transaction;
    readonly ILogger _logger;

    public SqlDbMigrator(SqlConnection connection, ILogger logger)
    {
        _connection = connection;
        _logger = logger;

        _transaction = _connection.BeginTransaction();
    }

    public void Dispose()
    {
        _transaction.Dispose();
    }

    public Task CommitAsync()
    {
        _transaction.Commit();

        return Task.CompletedTask;
    }

    public Task RollbackAsync()
    {
        _transaction.Rollback();

        return Task.CompletedTask;
    }
    
    public async Task PrepareAsync()
    {
        await _connection.ExecuteNonQueryCommandAsync(Scripts.EnsureMigrationTableExists, transaction: _transaction);
    }

    public async Task ApplyMigrationAsync(string filename, string migrationText)
    {
        try
        {
            await _connection.ExecuteNonQueryCommandAsync(migrationText, transaction: _transaction);
        }
        catch (SqlException ex)
        {
            _logger.Error($"Line {ex.LineNumber}: {ex.Message}");
            throw new DbMigrationException(filename, ex.LineNumber, "An error occured applying data migration", ex);
        }
    }

}
