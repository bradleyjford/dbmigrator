namespace DbMigrator.Cli;

interface IVerbHandler
{
    Task<int> ExecuteAsync();
}