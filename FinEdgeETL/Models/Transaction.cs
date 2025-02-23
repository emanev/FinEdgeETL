namespace FinEdgeETL.Models;

public class Transaction
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
}
