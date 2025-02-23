using FinEdgeETL.Pipeline;
using Microsoft.Extensions.Configuration;
using Serilog;

class Program
{
    static void Main()
    {       
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            Log.Information("ETL process started...");

            var pipeline = new ETLPipeline(configuration);
            pipeline.Run();

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
}
