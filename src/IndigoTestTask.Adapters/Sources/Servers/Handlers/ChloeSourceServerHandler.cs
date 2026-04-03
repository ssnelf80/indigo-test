using IndigoTestTask.Adapters.Sources.Dtos;
using IndigoTestTask.Adapters.SourceServers;

namespace IndigoTestTask.Adapters.Sources.Servers.Handlers;

public sealed class ChloeSourceServerHandler(SourceServerOptions options)
    : BaseSourceServerHandler<ChloeSourceDto>(options)
{
    protected override ChloeSourceDto GenerateMessage()
    {
        var price = Random.Next(10, 100_001) / 100m;
        return new ChloeSourceDto(
            Guid.NewGuid().ToString(),
            price,
            Random.Next(1, 1001) * price,
            DateTimeOffset.UtcNow);
    }
}