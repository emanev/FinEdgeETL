using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using FinEdgeETL.Models;
using Serilog;

namespace FinEdgeETL.DataExtraction;

public class CsvExtractor : IDataExtractor
{
    private readonly string _filePath;

    public CsvExtractor(string filePath)
    {
        _filePath = filePath;

        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException($"CSV file not found: {_filePath}");
        }
    }

    public IEnumerable<Transaction> ExtractData()
    {
        try
        {
            Log.Information($"Reading CSV file: {_filePath}");

            if (!File.Exists(_filePath))
            {
                Log.Warning($"CSV file not found: {_filePath}");
                return new List<Transaction>();
            }

            using var reader = new StreamReader(_filePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

            var records = csv.GetRecords<Transaction>().ToList();
            Log.Information($"Retrieved {records.Count} transactions from CSV.");
            return records;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while reading the CSV file.");
            return new List<Transaction>();
        }
    }

}
