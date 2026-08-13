using LoanApp.DTOs;

namespace LoanApp.Domain.Rules;

/// <summary>
/// Denies applications from the state of NY.
/// </summary>
public class StateRule : IRule
{
    public string Reason => "Applicants from NY are not allowed.";

    public bool IsValid(ApplicationRequest request) =>
        !string.Equals(request.State, "NY", StringComparison.OrdinalIgnoreCase);
}