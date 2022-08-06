using Microsoft.Data.SqlClient;

namespace DbMigrator.Core.SqlClient;

static class SqlConnectionExtensions
{
    public static SqlCommand CreateCommand(this SqlConnection connection,
        string commandText,
        SqlTransaction? transaction = null)
    {
        var command = connection.CreateCommand();

        command.Transaction = transaction;
        command.CommandText = commandText;

        return command;
    }

    public static async Task<int> ExecuteNonQueryCommandAsync(
        this SqlConnection connection, 
        string commandText, 
        object? arguments = null, 
        SqlTransaction? transaction = null)
    {
        using var command = CreateCommand(connection, commandText, transaction);
            
        return await command.ExecuteNonQueryAsync();
    }
}