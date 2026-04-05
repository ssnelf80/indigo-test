using IndigoTestTask.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IndigoTestTask.App;

public class TickProcessedChecker : IHostedService, IDisposable
{
    private readonly Timer _timer;
    private const int CheckProcessedCountTimeoutMs = 5_000;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<TickProcessedChecker> _logger;

    public TickProcessedChecker(IServiceScopeFactory serviceScopeFactory, ILogger<TickProcessedChecker> logger)
    {
        _timer = new Timer(CheckProcessedCount, null, Timeout.Infinite, Timeout.Infinite);;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public void Dispose()
    {
       _timer.Dispose();
       GC.SuppressFinalize(this);
    }

    private async void CheckProcessedCount(object? state)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITickRepository>();
            _logger.LogInformation("Processed items: {count}", await repository.CountAsync(CancellationToken.None));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "CheckProcessedCount error");
        }
        finally
        {
            _timer.Change(CheckProcessedCountTimeoutMs, Timeout.Infinite);
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer.Change(0, Timeout.Infinite);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Change(Timeout.Infinite, Timeout.Infinite);
        return Task.CompletedTask;
    }
}