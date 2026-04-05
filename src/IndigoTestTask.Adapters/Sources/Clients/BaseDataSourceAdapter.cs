using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using IndigoTestTask.Adapters.Sources.BaseTickConverter;
using IndigoTestTask.Adapters.Sources.Options;
using IndigoTestTask.Domain.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Registry;

namespace IndigoTestTask.Adapters.Sources.Clients;

public abstract class BaseDataSourceAdapter<T>(
    IDatabusPublisher databusPublisher,
    ResiliencePipelineProvider<string> pipelineProvider,
    BaseDomainTickConverter<T> domainTickConverter, 
    IOptions<BaseAdapterOptions> adapterOptions,
    ILogger logger) : BackgroundService
    where T : ITickDto
{
    private readonly ResiliencePipeline _pipeline = pipelineProvider.GetPipeline("ws-client");
    private readonly BaseAdapterOptions _options = adapterOptions.Value;
    protected abstract string AdapterName { get; }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await _pipeline.ExecuteAsync(InternalExecuteAsync, cancellationToken);
    }
    
    private async ValueTask InternalExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var client = new ClientWebSocket();
            await client.ConnectAsync(new Uri(_options.Url), cancellationToken);
            logger.LogInformation("Adapter {Name} connected", AdapterName);
            await using var stream = WebSocketStream.Create(client, WebSocketMessageType.Binary);
            var dataEnumerable = JsonSerializer.DeserializeAsyncEnumerable<T>(stream,
                topLevelValues: true,
                options: TickDtoJsonSerializerOptions.JsonSerializerOptions,
                cancellationToken: cancellationToken);
            await foreach (var item in dataEnumerable)
            {
                if (item is null)
                    throw new ArgumentNullException(nameof(item));
               
                var domainModel = domainTickConverter.ToDomainModel(item);
               
                await databusPublisher.PublishAsync(JsonSerializer.SerializeToUtf8Bytes(domainModel),
                    cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError("Adapter {Name} error: {Message}", AdapterName, ex.Message);
            throw;
        }
    }
}