using LoanApp.Infrastructure;
using LoanApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<LoanDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LoanDb")));
builder.Services.AddScoped<LoanService>();

builder.Services.AddSingleton<ApplicationChannel>();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<ApplicationPublisher>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDevelopment",
        policy => policy.WithOrigins(
                        "http://localhost:3000",        // Next.js frontend
                        "https://localhost:5001",       // Backend HTTPS
                        "https://localhost:7228",       // Development HTTPS variant
                        "http://localhost:5173",        // Vite dev server
                        "https://localhost:5173")       // Vite dev server HTTPS
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors("AllowDevelopment");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
