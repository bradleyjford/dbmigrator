namespace DbMigrator.Core;

public interface IDbMigrator : IDisposable
{
    Task CommitAsync();
    Task RollbackAsync();
    Task PrepareAsync();
    Task ApplyMigrationAsync(string filename, string migrationScript);
}
