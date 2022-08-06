using Microsoft.Data.SqlClient;

namespace DbMigrator.Core.SqlClient;

static class SqlConnectionExtensions
{
    public static SqlCommand CreateCommand(
        this SqlConnection connection, 
        string commandText, 
        object? arguments = null, 
        SqlTransaction? transaction = null)
    {
        var command = connection.CreateCommand();

        command.Transaction = transaction;
        command.CommandText = commandText;

        if (arguments != null)
        {
            var argumentDictionary = AnonymousTypeToDictionaryConverter.Convert(arguments);

            foreach (var pair in argumentDictionary)
            {
                command.Parameters.AddWithValue(pair.Key, pair.Value);
            }
        }

        return command;
    }

    public static async Task<int> ExecuteNonQueryCommandAsync(
        this SqlConnection connection, 
        string commandText, 
        object? arguments = null, 
        SqlTransaction? transaction = null)
    {
        using var command = CreateCommand(connection, commandText, arguments, transaction);
            
        return await command.ExecuteNonQueryAsync();
    }
}