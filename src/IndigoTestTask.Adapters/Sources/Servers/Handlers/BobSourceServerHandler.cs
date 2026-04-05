using IndigoTestTask.Adapters.Sources.Dtos;
using IndigoTestTask.Adapters.Sources.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IndigoTestTask.Adapters.Sources.Servers.Handlers;

public sealed class BobSourceServerHandler(IOptions<BobServiceOptions> options, ILogger<BobSourceServerHandler> logger)
    : BaseSourceServerHandler<BobSourceDto>(options, logger)
{
    protected override string Name => "Bob";

    protected override BobSourceDto GenerateMessage() =>
        new()
        {
            Ticker = Guid.NewGuid().ToString(),
            TotalPrice = Random.Next(10, 100_001) / 100m,
            Count = Random.Next(1, 1001)
        };
}