using IndigoTestTask.Adapters.Sources.Converters;
using IndigoTestTask.Adapters.Sources.Dtos;
using IndigoTestTask.Adapters.Sources.Options;
using IndigoTestTask.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Polly.Registry;

namespace IndigoTestTask.Adapters.Sources.Clients.Adapters;

public sealed class ChloeDataSourceAdapter(
    IDatabusPublisher databusPublisher,
    ResiliencePipelineProvider<string> pipelineProvider, 
    ChloeDomainTickConverter domainTickConverter, 
    ChloeAdapterOptions options,
    ILogger<ChloeDataSourceAdapter> logger) 
    : BaseDataSourceAdapter<ChloeSourceDto>(databusPublisher, pipelineProvider, domainTickConverter, options, logger)
{
    
}