using IndigoTestTask.Adapters.Sources.Clients;

namespace IndigoTestTask.Adapters.Sources.Options;

public class ChloeAdapterOptions : BaseAdapterOptions
{
    public override required string Url { get; init; }
}