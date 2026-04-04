using IndigoTestTask.Adapters.Sources.Dtos;
using IndigoTestTask.Adapters.Sources.Options;
using Microsoft.Extensions.Options;

namespace IndigoTestTask.Adapters.Sources.Servers.Handlers;

public sealed class BobSourceServerHandler(IOptions<BobServiceOptions> options)
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