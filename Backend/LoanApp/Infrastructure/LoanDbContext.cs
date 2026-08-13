using Microsoft.EntityFrameworkCore;
using LoanApp.Domain;

namespace LoanApp.Infrastructure;

public class LoanDbContext : DbContext
{
    public LoanDbContext(DbContextOptions<LoanDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Application> Applications => Set<Application>();
}
