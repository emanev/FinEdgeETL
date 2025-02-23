using FinEdgeETL.Models;

namespace FinEdgeETL.DataTransformation;

public class TransactionProcessor
{
    public List<Transaction> Process(IEnumerable<Transaction> transactions)
    {
        var processedData = transactions.AsParallel()
            .Where(t => t.Amount > 10)
            .GroupBy(t => t.CustomerId)
            .Select(g => g.OrderByDescending(t => t.TransactionDate).First())
            .ToList();

        return processedData;
    }
}
