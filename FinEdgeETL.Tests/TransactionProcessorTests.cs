using FinEdgeETL.DataTransformation;
using FinEdgeETL.Models;

namespace FinEdgeETL.Tests
{
    public class TransactionProcessorTests
    {
        [Fact]
        public void Process_ShouldFilterAndKeepLatestTransactionPerCustomer()
        {
            // Arrange
            var processor = new TransactionProcessor();
            var transactions = new List<Transaction>
            {
                new Transaction { Id = 1, CustomerId = 1001, Amount = 250.50m, TransactionDate = new DateTime(2024, 2, 20) },
                new Transaction { Id = 2, CustomerId = 1001, Amount = 300.75m, TransactionDate = new DateTime(2024, 2, 22) }, // Newer transaction
                new Transaction { Id = 3, CustomerId = 1002, Amount = 125.00m, TransactionDate = new DateTime(2024, 2, 21) },
                new Transaction { Id = 4, CustomerId = 1002, Amount = 90.00m, TransactionDate = new DateTime(2024, 2, 19) }, // Older transaction
                new Transaction { Id = 5, CustomerId = 1003, Amount = 8.00m, TransactionDate = new DateTime(2024, 2, 22) } // Below threshold
            };

            // Act
            var processedTransactions = processor.Process(transactions).ToList();

            // Assert
            Assert.Equal(2, processedTransactions.Count); // Only two customers with valid transactions
            Assert.Contains(processedTransactions, t => t.CustomerId == 1001 && t.Amount == 300.75m);
            Assert.Contains(processedTransactions, t => t.CustomerId == 1002 && t.Amount == 125.00m);
        }
    }
}
