using IndigoTestTask.Adapters.Sources.Clients;

namespace IndigoTestTask.Adapters.Sources.Options;

public class BobAdapterOptions : BaseAdapterOptions
{
    public override required string Url { get; init; }
}