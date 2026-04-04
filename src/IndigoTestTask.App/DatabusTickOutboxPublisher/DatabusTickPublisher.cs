using IndigoTestTask.DAL.Outbox;
using IndigoTestTask.Domain.Repositories;
using KafkaFlow.Producers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace IndigoTestTask.App.DatabusTickOutboxPublisher;

public class DatabusTickPublisher : IDatabusPublisher, IDisposable
{
    private const int CheckOutboxDatabaseTimeoutMs = 300;
    private const int CountOfWorkers = 8;
    private readonly Timer _timerPoll;
    private readonly IServiceScopeFactory _serviceProviderFactory;
    private readonly IReadOnlyCollection<OutboxTickPublishWorker> _outboxPublishWorkers;
    

    public DatabusTickPublisher(IProducerAccessor producerAccessor, IServiceScopeFactory serviceProviderFactory, ILogger<DatabusTickPublisher> logger, OutboxProvider outboxProvider)
    {
        _serviceProviderFactory = serviceProviderFactory;
        _timerPoll = new Timer(PollCallback, null, Timeout.Infinite, Timeout.Infinite);
        
        List<OutboxTickPublishWorker> workers = new();
        for(var i=0; i<CountOfWorkers;i++)
            workers.Add(new OutboxTickPublishWorker(producerAccessor, outboxProvider, logger));
        _outboxPublishWorkers = workers;
    }


    public async Task PublishAsync(byte[] message, CancellationToken cancellationToken)
    {
        using var scope = _serviceProviderFactory.CreateScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        await outboxRepository.SaveAsync(message, cancellationToken);
    }

    private void PollCallback(object? state)
    {
        try
        {
            foreach (var worker in _outboxPublishWorkers)
                worker.Publish();
        }
        finally
        {
            _timerPoll.Change(CheckOutboxDatabaseTimeoutMs, Timeout.Infinite);
        }
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timerPoll.Change(0, Timeout.Infinite);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timerPoll.Change(Timeout.Infinite, Timeout.Infinite);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timerPoll.Dispose();
    }
}