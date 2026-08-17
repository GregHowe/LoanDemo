using LoanApp.DTOs;
using LoanApp.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

public class ApplicationPublisher : BackgroundService
{
    private readonly ApplicationChannel _channel;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ApplicationPublisher> _logger;
    private readonly string _externalServiceUrl;
    private readonly bool _isDevelopment;

    public ApplicationPublisher(
        ApplicationChannel channel,
        IHttpClientFactory httpClientFactory,
        ILogger<ApplicationPublisher> logger,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        _channel = channel;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _isDevelopment = environment.IsDevelopment();
        _externalServiceUrl = configuration["ExternalService:BaseUrl"] ?? throw new ArgumentNullException("ExternalService:BaseUrl");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var handler = new HttpClientHandler();
        if (_isDevelopment)
        {
            handler.ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true;
        }
        
        var client = new HttpClient(handler);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_channel.TryDequeue(out var request) && request != null)
            {
                try
                {
                    // Check if customer already exists (returning customer)
                    var checkResponse = await client.GetAsync($"{_externalServiceUrl}/mock/applications/{request.SSN}", stoppingToken);

                    if (checkResponse.IsSuccessStatusCode)
                    {
                        // Customer exists → UPDATE with PUT
                        var updateResponse = await client.PutAsJsonAsync(
                            $"{_externalServiceUrl}/mock/applications/{request.SSN}",
                            request,
                            stoppingToken);

                        if (!updateResponse.IsSuccessStatusCode)
                        {
                            _logger.LogWarning("Failed to update application in external service. Status: {StatusCode}", updateResponse.StatusCode);
                        }
                        else
                        {
                            _logger.LogInformation("Customer updated in external service: {SSN}", request.SSN);
                        }
                    }
                    else if (checkResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        // Customer doesn't exist → CREATE with POST
                        var createResponse = await client.PostAsJsonAsync(
                            $"{_externalServiceUrl}/mock/applications",
                            request,
                            stoppingToken);

                        if (!createResponse.IsSuccessStatusCode)
                        {
                            _logger.LogWarning("Failed to create application in external service. Status: {StatusCode}", createResponse.StatusCode);
                        }
                        else
                        {
                            _logger.LogInformation("Customer created in external service: {SSN}", request.SSN);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending application to external service.");
                }
            }
            else
            {
                await Task.Delay(500, stoppingToken);
            }
        }
    }
}