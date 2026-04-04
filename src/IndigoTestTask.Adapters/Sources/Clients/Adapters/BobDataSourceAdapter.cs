using IndigoTestTask.Adapters.Sources.Converters;
using IndigoTestTask.Adapters.Sources.Dtos;
using IndigoTestTask.Adapters.Sources.Options;
using IndigoTestTask.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Registry;

namespace IndigoTestTask.Adapters.Sources.Clients.Adapters;

public sealed class BobDataSourceAdapter(
    IDatabusPublisher databusPublisher,
    ResiliencePipelineProvider<string> pipelineProvider, 
    BobDomainTickConverter domainTickConverter, 
    IOptions<BobAdapterOptions> options,
    ILogger<BobDataSourceAdapter> logger) 
    : BaseDataSourceAdapter<BobSourceDto>(databusPublisher, pipelineProvider, domainTickConverter, options, logger)
{
    protected override string AdapterName => "Bob";
}