using IndigoTestTask.Adapters.Sources.Clients;

namespace IndigoTestTask.Adapters.Sources.Options;

public class AliceAdapterOptions : BaseAdapterOptions
{
    public override required string Url { get; init; } = "ws://localhost:5054/ws-alice";
}