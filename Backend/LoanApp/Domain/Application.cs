namespace LoanApp.Domain;

public class Application
{
    public int Id { get; set; }
    public decimal RequestedAmount { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}