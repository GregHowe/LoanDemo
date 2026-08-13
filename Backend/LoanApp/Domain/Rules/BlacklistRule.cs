using LoanApp.DTOs;

namespace LoanApp.Domain.Rules;

/// <summary>
/// Denies applications with blacklisted SSNs.
/// </summary>
public class BlacklistRule : IRule
{
    // Example: SSNs starting with "666" are blacklisted.
    public string Reason => "SSN is blacklisted.";

    public bool IsValid(ApplicationRequest request) =>
        !request.SSN.StartsWith("666");
}