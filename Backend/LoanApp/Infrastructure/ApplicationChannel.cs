using LoanApp.DTOs;
using System.Threading.Channels;

namespace LoanApp.Infrastructure;

public class ApplicationChannel
{
    private readonly Channel<ApplicationRequest> _channel = Channel.CreateUnbounded<ApplicationRequest>();

    public async Task EnqueueAsync(ApplicationRequest request)
    {
        await _channel.Writer.WriteAsync(request);
    }

    public bool TryDequeue(out ApplicationRequest? request)
    {
        return _channel.Reader.TryRead(out request);
    }
}
