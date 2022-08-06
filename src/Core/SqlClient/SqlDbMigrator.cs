using Microsoft.Data.SqlClient;

namespace DbMigrator.Core.SqlClient;

public class SqlDbMigrator : IDbMigrator
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
    
    public async Task EnsureSchemaMigrationTableExistsAsync()
    {
        await _connection.ExecuteNonQueryCommandAsync(Scripts.EnsureMigrationTableExists, transaction: _transaction);
    }

    public async Task ApplyMigrationAsync(string migrationText)
    {
        try
        {
            await _connection.ExecuteNonQueryCommandAsync(migrationText, transaction: _transaction);
        }
        catch (SqlException ex)
        {
            _logger.Error($"Line {ex.LineNumber}: {ex.Message}");
            throw ex;
        }
    }

}