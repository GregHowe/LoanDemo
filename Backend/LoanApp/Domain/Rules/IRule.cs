using LoanApp.DTOs;

namespace LoanApp.Domain.Rules;

/// <summary>
/// Interface for business rules applied to loan applications.
/// </summary>
public interface IRule
{
    /// <summary>
    /// Returns true if the request passes the rule.
    /// </summary>
    bool IsValid(ApplicationRequest request);

    /// <summary>
    /// The reason for denial if the rule is not satisfied.
    /// </summary>
    string Reason { get; }
}