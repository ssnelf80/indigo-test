using IndigoTestTask.Adapters.Sources.Dtos;
using IndigoTestTask.Adapters.Sources.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IndigoTestTask.Adapters.Sources.Servers.Handlers;

public sealed class ChloeSourceServerHandler(IOptions<ChloeServiceOptions> options, ILogger<ChloeSourceServerHandler> logger)
    : BaseSourceServerHandler<ChloeSourceDto>(options, logger)
{
    protected override string Name => "Chloe";

    protected override ChloeSourceDto GenerateMessage()
    {
        var price = Random.Next(10, 100_001) / 100m;
        return new ChloeSourceDto
        {
            Ticker = Guid.NewGuid().ToString(),
            Price = price,
            Volume = Random.Next(1, 1001) * price,
            Timestamp = DateTimeOffset.Now
        };
    }
}