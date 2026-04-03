using System.Diagnostics;
using System.Net.WebSockets;
using System.Text.Json;
using IndigoTestTask.Adapters.Sources.Options;
using IndigoTestTask.Adapters.SourceServers;
using IndigoTestTask.Domain.Services.BaseTickConverter;

namespace IndigoTestTask.Adapters.Sources.Servers;

public abstract class BaseSourceServerHandler<T>(SourceServerOptions options) where T : ITickDto
{
    private volatile bool _isUnavailable = false;
    private readonly Stopwatch _stopwatch = new();
    protected readonly Random Random = new();
    
    public async Task Handle(WebSocket webSocket, CancellationToken cancellationToken)
    {
        if (_isUnavailable)
        {
            webSocket.Abort();
            return;
        }

        await using var stream = WebSocketStream.Create(webSocket, WebSocketMessageType.Binary);
        
        while (true)
        {
            _stopwatch.Restart();
            for (var i = 0; i < options.Rps; ++i)
            {
                if (IsShouldAbort())
                {
                    webSocket.Abort();
                    return;
                }
                var message = GetSerializedMessage();
                await stream.WriteAsync(message, cancellationToken);

                if (options.CanSendDuplicate && IsRandomEvent(0.05))
                    await stream.WriteAsync(message, cancellationToken);
            }
            
            await stream.FlushAsync(cancellationToken);
            _stopwatch.Stop();
            if (_stopwatch.Elapsed < TimeSpan.FromSeconds(1))
            {
                await Task.Delay(TimeSpan.FromSeconds(1) - _stopwatch.Elapsed, cancellationToken);
            }
           
        }
    }
    
    private byte[] GetSerializedMessage()
    {
        var message = GenerateMessage();
        return JsonSerializer.SerializeToUtf8Bytes(message, TickDtoJsonSerializerOptions.JsonSerializerOptions);
    }
    protected abstract T GenerateMessage();

    private bool IsShouldAbort() => IsLongTimeAbort() && IsSingleTimeAbort();
    private bool IsRandomEvent(double percentage) => Random.NextDouble() <= percentage;

    private bool IsLongTimeAbort()
    {
        if (!options.CanLongTimeAbort || IsRandomEvent(0.005))
            return false;
        
        _isUnavailable = true;
        Task.Run(async () =>
        {
            await Task.Delay(5_000);
            Interlocked.Exchange(ref _isUnavailable, false);
        });
        return true;
    }

    private bool IsSingleTimeAbort()
    {
        if (!options.CanSingleTimeAbort || IsRandomEvent(0.01))
            return false;

        return true;
    }
    
    
}