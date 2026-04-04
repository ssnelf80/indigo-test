using IndigoTestTask.Adapters.Sources.Dtos;
using IndigoTestTask.Adapters.Sources.Options;
using Microsoft.Extensions.Options;

namespace IndigoTestTask.Adapters.Sources.Servers.Handlers;

public sealed class ChloeSourceServerHandler(IOptions<ChloeServiceOptions> options)
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