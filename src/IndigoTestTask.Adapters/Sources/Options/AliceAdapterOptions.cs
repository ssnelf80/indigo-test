using IndigoTestTask.Adapters.Sources.Clients;

namespace IndigoTestTask.Adapters.Sources.Options;

public class AliceAdapterOptions : BaseAdapterOptions
{
    public override required string Url { get; init; }
}