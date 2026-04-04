using IndigoTestTask.DAL.Outbox;
using IndigoTestTask.DAL.Outbox.Entities;
using KafkaFlow;
using KafkaFlow.Producers;
using Microsoft.Extensions.Logging;

namespace IndigoTestTask.App.DatabusTickOutboxPublisher;

public class OutboxTickPublishWorker(IProducerAccessor producerAccessor, OutboxProvider outboxProvider, ILogger logger)
{
    private const string OutboxTopic = "tick-queue";
    private readonly IMessageProducer _producer = producerAccessor.GetProducer("outbox-producer");
    private bool _isProcessing = false;

    public void Publish()
    {
        if (Interlocked.CompareExchange(ref _isProcessing, true, false))
            return;

        Task.Run(async () =>
        {
            try
            {
                while (await outboxProvider.ProcessAsync(PublishAsync, CancellationToken.None))
                {
                
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "An error occurred while outbox processing");
            }
            finally
            {
                Interlocked.Exchange(ref _isProcessing, false);
            }
        });

    }
    
    private async Task PublishAsync(OutboxMessage[] messages, CancellationToken cancellationToken)
    {
        foreach (var message in messages)
            await _producer.ProduceAsync(OutboxTopic, message.Id.ToString(), message.Message);
    }
}