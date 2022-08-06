namespace DbMigrator.Core;

public interface IDbMigrator : IDisposable
{
    Task CommitAsync();
    Task RollbackAsync();
    Task EnsureSchemaMigrationTableExistsAsync();
    Task ApplyMigrationAsync(string filename, string migrationScript);
}