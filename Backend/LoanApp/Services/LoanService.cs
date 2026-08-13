using LoanApp.Domain;
using LoanApp.Domain.Rules;
using LoanApp.DTOs;
using LoanApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LoanApp.Services;

public class LoanService
{
    private readonly LoanDbContext _dbContext;
    private readonly ApplicationChannel _channel;
    private readonly List<IRule> _rules;

    public LoanService(LoanDbContext context, ApplicationChannel channel)
    {
        _dbContext = context;
        _channel = channel;
        _rules = new List<IRule> { new StateRule(), new BlacklistRule() };
    }

    public async Task<ApplicationResult> ProcessApplicationAsync(ApplicationRequest request)
    {
        // Evaluate rules using the rules engine
        foreach (var rule in _rules)
        {
            if (!rule.IsValid(request))
            {
                return new ApplicationResult
                {
                    Approved = false,
                    Reason = rule.Reason
                };
            }
        }

        var supportsTransactions = _dbContext.Database.IsRelational();
        IDbContextTransaction? transaction = null;

        if (supportsTransactions)
        {
            transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        try
        {
            // Check if the customer already exists (Returning customer)
            var existingCustomer = await _dbContext.Customers
                .FirstOrDefaultAsync(c => c.SSN == request.SSN);

            Customer customer;
            if (existingCustomer != null)
            {
                // Update
                existingCustomer.FirstName = request.FirstName;
                existingCustomer.LastName = request.LastName;
                existingCustomer.Address = request.Address;
                existingCustomer.State = request.State;
                existingCustomer.CompanyName = request.CompanyName;
                customer = existingCustomer;
            }
            else
            {
                // Insert
                customer = new Customer
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Address = request.Address,
                    State = request.State,
                    CompanyName = request.CompanyName,
                    SSN = request.SSN
                };
                _dbContext.Customers.Add(customer);
            }

            var existingApplication = await _dbContext.Applications
                .FirstOrDefaultAsync(a => a.CustomerId == customer.Id);

            if (existingApplication != null)
            {
                existingApplication.RequestedAmount = request.RequestedAmount;
            }
            else
            {
                var application = new Application
                {
                    RequestedAmount = request.RequestedAmount,
                    Customer = customer
                };

                _dbContext.Applications.Add(application);
            }

            await _dbContext.SaveChangesAsync();

            // Publish to the channel
            await _channel.EnqueueAsync(request);

            if (transaction != null)
            {
                await transaction.CommitAsync();
            }

            return new ApplicationResult { Approved = true };
        }
        catch
        {
            if (transaction != null)
            {
                await transaction.RollbackAsync();
            }

            throw;
        }
    }
}