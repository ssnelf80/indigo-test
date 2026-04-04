using IndigoTestTask.Adapters.Sources.Converters;
using IndigoTestTask.Adapters.Sources.Dtos;
using IndigoTestTask.Adapters.Sources.Options;
using IndigoTestTask.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Registry;

namespace IndigoTestTask.Adapters.Sources.Clients.Adapters;

public sealed class AliceDataSourceAdapter(
    IDatabusPublisher databusPublisher,
    ResiliencePipelineProvider<string> pipelineProvider, 
    AliceDomainTickConverter domainTickConverter, 
    IOptions<AliceAdapterOptions> options,
    ILogger<AliceDataSourceAdapter> logger) 
    : BaseDataSourceAdapter<AliceSourceDto>(databusPublisher, pipelineProvider, domainTickConverter, options, logger)
{
    protected override string AdapterName => "Alice";
}