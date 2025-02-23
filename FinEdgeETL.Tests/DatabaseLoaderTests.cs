using FinEdgeETL.DataLoading;
using FinEdgeETL.Models;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Data;

namespace FinEdgeETL.Tests
{
    public class DatabaseLoaderTests
    {
        [Fact]
        public void LoadData_ShouldNotThrowException_WhenValidDataProvided()
        {
            // Arrange
            var mockConnection = new Mock<IDbConnection>();
            var loader = new DatabaseLoader("FakeConnectionString"); // Simulating without actual DB

            var transactions = new List<Transaction>
            {
                new Transaction { Id = 1, CustomerId = 1001, Amount = 250.50m, TransactionDate = System.DateTime.Now },
                new Transaction { Id = 2, CustomerId = 1002, Amount = 125.00m, TransactionDate = System.DateTime.Now }
            };

            // Act & Assert
            var exception = Record.Exception(() => loader.LoadData(transactions));
            Assert.Null(exception); // Test passes if no exception is thrown
        }
    }
}
