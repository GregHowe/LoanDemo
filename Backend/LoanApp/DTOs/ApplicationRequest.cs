namespace LoanApp.DTOs;


public class ApplicationRequest
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Address { get; set; } = "";
    public string State { get; set; } = "";
    public string CompanyName { get; set; } = "";
    public decimal RequestedAmount { get; set; }
    public string SSN { get; set; } = "";
}
