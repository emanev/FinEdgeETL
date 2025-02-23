using System.Data;
using Dapper;
using FinEdgeETL.Models;
using Microsoft.Data.SqlClient;
using Serilog;

namespace FinEdgeETL.DataExtraction;

public class DatabaseExtractor : IDataExtractor
{
    private readonly string _connectionString;

    public DatabaseExtractor(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<Transaction> ExtractData()
    {
        try
        {
            Log.Information("Extracting data from SourceDB...");

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var query = "SELECT Id, CustomerId, Amount, TransactionDate FROM SourceTransactions";
            var transactions = connection.Query<Transaction>(query).ToList();

            Log.Information($"Retrieved {transactions.Count} transactions from SourceDB.");
            return transactions;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while extracting data from SourceDB.");
            return new List<Transaction>();
        }
    }
}
