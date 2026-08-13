using LoanApp.Controllers;
using LoanApp.Domain;
using LoanApp.Domain.Rules;
using LoanApp.DTOs;
using LoanApp.Infrastructure;
using LoanApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LoanApp.Tests;

public class RuleEngineTests
{
    [Fact]
    public void StateRule_Denies_New_York_Applications()
    {
        var rule = new StateRule();
        var request = new ApplicationRequest { State = "NY" };

        Assert.False(rule.IsValid(request));
        Assert.Equal("Applicants from NY are not allowed.", rule.Reason);
    }

    [Fact]
    public void StateRule_Allows_Non_New_York_Applications()
    {
        var rule = new StateRule();
        var request = new ApplicationRequest { State = "CA" };

        Assert.True(rule.IsValid(request));
    }

    [Fact]
    public void BlacklistRule_Denies_Blacklisted_SSNs()
    {
        var rule = new BlacklistRule();
        var request = new ApplicationRequest { SSN = "666123456" };

        Assert.False(rule.IsValid(request));
        Assert.Equal("SSN is blacklisted.", rule.Reason);
    }

    [Fact]
    public void BlacklistRule_Allows_Allowed_SSNs()
    {
        var rule = new BlacklistRule();
        var request = new ApplicationRequest { SSN = "123456789" };

        Assert.True(rule.IsValid(request));
    }
}

public class LoanServiceTests
{
    private static LoanService CreateService(LoanDbContext dbContext)
    {
        return new LoanService(dbContext, new ApplicationChannel());
    }

    [Fact]
    public async Task ProcessApplicationAsync_WhenCustomerDoesNotExist_CreatesCustomerAndApplication()
    {
        var options = new DbContextOptionsBuilder<LoanDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var dbContext = new LoanDbContext(options);
        var service = CreateService(dbContext);
        var request = new ApplicationRequest
        {
            FirstName = "Juan",
            LastName = "Perez",
            Address = "Av. Siempre Viva 123",
            State = "CA",
            CompanyName = "TechCorp",
            RequestedAmount = 5000m,
            SSN = "123456789"
        };

        var result = await service.ProcessApplicationAsync(request);

        Assert.True(result.Approved);
        Assert.Equal(1, await dbContext.Customers.CountAsync());
        Assert.Equal(1, await dbContext.Applications.CountAsync());

        var customer = await dbContext.Customers.SingleAsync();
        Assert.Equal(request.SSN, customer.SSN);

        var application = await dbContext.Applications.SingleAsync();
        Assert.Equal(request.RequestedAmount, application.RequestedAmount);
        Assert.Equal(customer.Id, application.CustomerId);
    }

    [Fact]
    public async Task ProcessApplicationAsync_WhenCustomerAlreadyExists_UpdatesExistingRecord_InsteadOfCreatingDuplicate()
    {
        var options = new DbContextOptionsBuilder<LoanDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var dbContext = new LoanDbContext(options);
        var customer = new Customer
        {
            FirstName = "Juan",
            LastName = "Perez",
            Address = "Av. Siempre Viva 123",
            State = "CA",
            CompanyName = "TechCorp",
            SSN = "123456789"
        };

        dbContext.Customers.Add(customer);
        dbContext.Applications.Add(new Application
        {
            RequestedAmount = 5000m,
            Customer = customer,
            CreatedAt = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var service = CreateService(dbContext);
        var request = new ApplicationRequest
        {
            FirstName = "Juan",
            LastName = "Perez",
            Address = "Av. Siempre Viva 456",
            State = "CA",
            CompanyName = "TechCorp Updated",
            RequestedAmount = 15000m,
            SSN = "123456789"
        };

        var result = await service.ProcessApplicationAsync(request);

        Assert.True(result.Approved);
        Assert.Equal(1, await dbContext.Customers.CountAsync());
        Assert.Equal(1, await dbContext.Applications.CountAsync());

        var updatedCustomer = await dbContext.Customers.SingleAsync();
        Assert.Equal("TechCorp Updated", updatedCustomer.CompanyName);
        Assert.Equal("Av. Siempre Viva 456", updatedCustomer.Address);

        var updatedApplication = await dbContext.Applications.SingleAsync();
        Assert.Equal(15000m, updatedApplication.RequestedAmount);
        Assert.Equal(updatedCustomer.Id, updatedApplication.CustomerId);
    }
}

public class ApplicationsControllerTests
{
    [Fact]
    public async Task PostApplication_WhenApproved_ReturnsOkResult()
    {
        var options = new DbContextOptionsBuilder<LoanDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var dbContext = new LoanDbContext(options);
        var controller = new ApplicationsController(new LoanService(dbContext, new ApplicationChannel()));
        var request = new ApplicationRequest
        {
            FirstName = "Maria",
            LastName = "Lopez",
            Address = "100 Lake St",
            State = "TX",
            CompanyName = "Nova",
            RequestedAmount = 3000m,
            SSN = "111223333"
        };

        var result = await controller.PostApplication(request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task PostApplication_WhenDenied_ReturnsBadRequestResult()
    {
        var options = new DbContextOptionsBuilder<LoanDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var dbContext = new LoanDbContext(options);
        var controller = new ApplicationsController(new LoanService(dbContext, new ApplicationChannel()));
        var request = new ApplicationRequest
        {
            FirstName = "John",
            LastName = "Smith",
            Address = "123 Main St",
            State = "NY",
            CompanyName = "Acme",
            RequestedAmount = 7000m,
            SSN = "222334444"
        };

        var result = await controller.PostApplication(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var payload = Assert.IsType<ApplicationResult>(badRequest.Value);
        Assert.False(payload.Approved);
        Assert.Equal("Applicants from NY are not allowed.", payload.Reason);
    }
}
