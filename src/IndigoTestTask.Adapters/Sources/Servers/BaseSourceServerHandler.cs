using System.Diagnostics;
using System.Net.WebSockets;
using System.Text.Json;
using IndigoTestTask.Adapters.Sources.BaseTickConverter;
using IndigoTestTask.Adapters.Sources.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IndigoTestTask.Adapters.Sources.Servers;

public abstract class BaseSourceServerHandler<T> : IDisposable where T : ITickDto
{
    private volatile bool _isUnavailable = false;
    private bool _isSingleTimeAbort = false;
    private bool _isDuplicateMessage = false;
    private bool _isInvalidMessage = false;
    private readonly Timer _unavailableTimer;
    private readonly Timer _singleTimeAbortTimer;
    private readonly Timer _duplicateMessageTimer;
    private readonly Timer _invalidMessageTimer;
    private readonly ILogger<BaseSourceServerHandler<T>> _logger;
    private readonly byte[] _invalidMessage = JsonSerializer.SerializeToUtf8Bytes(new {Invalid = true}, TickDtoJsonSerializerOptions.JsonSerializerOptions);
    protected abstract string Name { get; }
    
    private readonly Stopwatch _stopwatch = new();
    protected readonly Random Random = new();
    private readonly BaseSourceServerOptions _options;

    protected BaseSourceServerHandler(IOptions<BaseSourceServerOptions> serverOptions, ILogger<BaseSourceServerHandler<T>> logger)
    {
        _logger = logger;
        _options = serverOptions.Value;
        _unavailableTimer = new Timer(LongTimeAbortTimerCallback, 
            null, 
            _options.UnavailableIntervalSec < 1 ? Timeout.Infinite : _options.UnavailableIntervalSec * 1_000, 
            Timeout.Infinite);
        _singleTimeAbortTimer = new Timer(SingleTimeAbortTimerCallback, 
            null, 
            _options.SingleTimeAbortIntervalSec < 1 ? Timeout.Infinite : _options.SingleTimeAbortIntervalSec * 1_000, 
            Timeout.Infinite);
        _duplicateMessageTimer = new Timer(DuplicateMessageTimerCallback, 
            null, 
            _options.SendDuplicateIntervalSec < 1 ? Timeout.Infinite : _options.SendDuplicateIntervalSec * 1_000, 
            Timeout.Infinite);
        _invalidMessageTimer = new Timer(InvalidMessageTimerCallback, 
            null, 
            _options.SendInvalidMessageIntervalSec < 1 ? Timeout.Infinite : _options.SendInvalidMessageIntervalSec * 1_000, 
            Timeout.Infinite);
    }

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
            for (var i = 0; i < _options.Rps; ++i)
            {
                if (_isUnavailable || Interlocked.CompareExchange(ref _isSingleTimeAbort, false, true))
                {
                    webSocket.Abort();
                    return;
                }

                var message = GetSerializedMessage();
                await stream.WriteAsync(message, cancellationToken);
                
                if (Interlocked.CompareExchange(ref _isDuplicateMessage, false, true))
                    await stream.WriteAsync(message, cancellationToken);

                if (Interlocked.CompareExchange(ref _isInvalidMessage, false, true))
                    await stream.WriteAsync(_invalidMessage, cancellationToken);
            }

            await stream.FlushAsync(cancellationToken);
            
            _stopwatch.Stop();
            
            if (_stopwatch.Elapsed < TimeSpan.FromSeconds(1)) 
                await Task.Delay(TimeSpan.FromSeconds(1) - _stopwatch.Elapsed, cancellationToken);
        }
    }

    private byte[] GetSerializedMessage() =>
        JsonSerializer.SerializeToUtf8Bytes(GenerateMessage(), TickDtoJsonSerializerOptions.JsonSerializerOptions);

    protected abstract T GenerateMessage();
    
    public void Dispose()
    {
        _unavailableTimer.Dispose();
        _singleTimeAbortTimer.Dispose();
        _invalidMessageTimer.Dispose();
        _duplicateMessageTimer.Dispose();
        GC.SuppressFinalize(this);
    }

    private void SingleTimeAbortTimerCallback(object? state)
    {
        try
        {
            if (!Interlocked.CompareExchange(ref _isSingleTimeAbort, true, false))
                 _logger.LogInformation("Adapter {Name} in single time abort mode", Name);
        }
        finally
        {
            _singleTimeAbortTimer.Change(_options.SingleTimeAbortIntervalSec * 1_000, Timeout.Infinite);
        }
    }

    private async void LongTimeAbortTimerCallback(object? state)
    {
        try
        {
            _logger.LogWarning("Adapter {Name} in unavailable mode", Name);
            Interlocked.Exchange(ref _isUnavailable, true);
            await Task.Delay(10_000);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
        finally
        {
            Interlocked.Exchange(ref _isUnavailable, false);
            _logger.LogWarning("Adapter {Name} available", Name);
            _unavailableTimer.Change(_options.UnavailableIntervalSec * 1_000, Timeout.Infinite);
        }
    }

    private void InvalidMessageTimerCallback(object? state)
    {
        try
        {
            if (!Interlocked.CompareExchange(ref _isInvalidMessage, true, false))
                _logger.LogWarning("Adapter {Name} in invalid message mode", Name);
        }
        finally
        {
            _invalidMessageTimer.Change(_options.SendInvalidMessageIntervalSec * 1_000, Timeout.Infinite);
        }
    }

    private void DuplicateMessageTimerCallback(object? state)
    {
        try
        {
            if (!Interlocked.CompareExchange(ref _isDuplicateMessage, true, false))
                _logger.LogWarning("Adapter {Name} in duplicate message mode", Name);
        }
        finally
        {
            _duplicateMessageTimer.Change(_options.SendDuplicateIntervalSec * 1_000, Timeout.Infinite);
        }
    }
}