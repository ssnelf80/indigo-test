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
    private volatile bool _isProcessing = false;
    private readonly Lock _sync = new();

    public void Publish()
    {
        lock (_sync)
        {
            if (_isProcessing)
                return;
            
            _isProcessing = true;
        }

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
                lock (_sync)
                {
                    _isProcessing = false;
                }
            }
        });

    }
    
    private async Task PublishAsync(OutboxMessage[] messages, CancellationToken cancellationToken)
    {
        foreach (var message in messages)
            await _producer.ProduceAsync(OutboxTopic, message.Id.ToString(), message.Message);
    }
    
}