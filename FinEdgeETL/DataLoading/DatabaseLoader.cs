using System.Data;
using Microsoft.Data.SqlClient;
using FinEdgeETL.Models;
using Dapper;
using Serilog;

namespace FinEdgeETL.DataLoading;

public class DatabaseLoader
{
    private readonly string _connectionString;

    public DatabaseLoader(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void LoadData(IEnumerable<Transaction> transactions)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var dataTable = ConvertToDataTable(transactions);
            Log.Information($"Loading {dataTable.Rows.Count} transactions into the database...");

            var parameters = new DynamicParameters();
            parameters.Add("@TransactionTable", dataTable.AsTableValuedParameter("TransactionType"));

            connection.Execute("UpsertTransaction", parameters, commandType: CommandType.StoredProcedure);
            Log.Information("Data successfully loaded into the database.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while loading data into the database.");
        }
    }

    private static DataTable ConvertToDataTable(IEnumerable<Transaction> transactions)
    {
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id", typeof(int));
        dataTable.Columns.Add("CustomerId", typeof(int));
        dataTable.Columns.Add("Amount", typeof(decimal));
        dataTable.Columns.Add("TransactionDate", typeof(DateTime));

        foreach (var transaction in transactions)
        {
            dataTable.Rows.Add(transaction.Id, transaction.CustomerId, transaction.Amount, transaction.TransactionDate);
        }

        return dataTable;
    }
}
