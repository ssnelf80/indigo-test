using IndigoTestTask.Adapters.Sources.Dtos;

namespace IndigoTestTask.Adapters.Sources.Servers.Handlers;

public sealed class BobSourceServerHandler(SourceServerOptions options)
    : BaseSourceServerHandler<BobSourceDto>(options)
{
    protected override BobSourceDto GenerateMessage()
    {
        return new BobSourceDto(
            Guid.NewGuid().ToString(),
            Random.Next(10, 100_001) / 100m,
            Random.Next(1, 1001));
    }
}