using IndigoTestTask.Domain.Entities;
using IndigoTestTask.Domain.Repositories;
using KafkaFlow;
using Microsoft.Extensions.DependencyInjection;

namespace IndigoTestTask.App.DatabusTickConsumer;

public class DatabusTickBatchConsumerHandler(IServiceScopeFactory scopeFactory) : IMessageMiddleware
{
    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        var ticks = context.GetMessagesBatch()
            .Select(x => (Tick)x.Message.Value).ToHashSet();
        
        using var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITickRepository>();
        await repository.AddTicksAsync(ticks, context.ConsumerContext.WorkerStopped);
    }
}