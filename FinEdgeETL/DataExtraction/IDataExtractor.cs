using FinEdgeETL.Models;

namespace FinEdgeETL.DataExtraction;

public interface IDataExtractor
{
    IEnumerable<Transaction> ExtractData();
}
