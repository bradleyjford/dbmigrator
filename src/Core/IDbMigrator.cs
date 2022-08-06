namespace DbMigrator.Core;

public interface IDbMigrationManager : IAsyncDisposable
{
    Task BackupDatabaseAsync(string filename);
    Task RecreateDatabaseAsync();
    IDbMigrator CreateMigrator();
}