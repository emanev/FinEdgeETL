using System.Threading.Tasks;
using FinEdgeETL.DataExtraction;
using FinEdgeETL.DataTransformation;
using FinEdgeETL.DataLoading;
using FinEdgeETL.Models;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace FinEdgeETL.Pipeline;

public class ETLPipeline
{
    private readonly List<IDataExtractor> _extractors;
    private readonly TransactionProcessor _processor;
    private readonly DatabaseLoader _loader;

    public ETLPipeline(IConfiguration configuration)
    {
        _extractors = new List<IDataExtractor>();
        
        string? csvFilePath = configuration["ETL:CsvFilePath"];
        if (!string.IsNullOrEmpty(csvFilePath))
        {
            _extractors.Add(new CsvExtractor(csvFilePath));
        }
        else
        {
            Log.Warning("CSV file path is missing in configuration. Skipping CSV extraction.");
        }
        
        string? sourceDbConnectionString = configuration.GetConnectionString("SourceDBConnection");
        if (!string.IsNullOrEmpty(sourceDbConnectionString))
        {
            _extractors.Add(new DatabaseExtractor(sourceDbConnectionString));
        }
        else
        {
            Log.Warning("Source database connection string is missing in configuration. Skipping database extraction.");
        }        
                
        if (!_extractors.Any())
        {
            throw new Exception("No valid data sources found in configuration. ETL cannot proceed.");
        }

        _processor = new TransactionProcessor();
        _loader = new DatabaseLoader(configuration.GetConnectionString("DefaultConnection")
            ?? throw new Exception("Default database connection string is missing in configuration."));
    }


    public void Run()
    {
        try
        {
            Log.Information("ETL process started...");
                        
            var extractTasks = _extractors.Select(extractor => Task.Run(() => extractor.ExtractData())).ToList();
            Task.WhenAll(extractTasks).Wait();

            var allTransactions = extractTasks.SelectMany(task => task.Result).ToList();
            Log.Information($"Data extraction completed. {allTransactions.Count} transactions retrieved.");

            Log.Information("Processing data in parallel...");

            var processedData = _processor.Process(allTransactions);

            Log.Information($"Data processing completed. {processedData.Count} transactions ready for loading.");
                        
            Log.Information("Loading data into the database...");
            Parallel.ForEach(SplitIntoBatches(processedData, batchSize: 1000), batch =>
            {
                _loader.LoadData(batch.AsEnumerable());
            });

            Log.Information("ETL process completed successfully.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error during ETL execution.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IEnumerable<List<Transaction>> SplitIntoBatches(IEnumerable<Transaction> transactions, int batchSize)
    {
        return transactions.Select((transaction, index) => new { transaction, index })
                           .GroupBy(x => x.index / batchSize)
                           .Select(g => g.Select(x => x.transaction).ToList());
    }
}
