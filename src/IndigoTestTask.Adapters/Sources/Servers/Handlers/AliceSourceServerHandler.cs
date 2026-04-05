using System.Globalization;
using IndigoTestTask.Adapters.Sources.Dtos;
using IndigoTestTask.Adapters.Sources.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IndigoTestTask.Adapters.Sources.Servers.Handlers;

public sealed class AliceSourceServerHandler(IOptions<AliceServiceOptions> options, ILogger<AliceSourceServerHandler> logger)
    : BaseSourceServerHandler<AliceSourceDto>(options, logger)
{
    protected override string Name => "Alice";

    protected override AliceSourceDto GenerateMessage()
    {
        var price = Random.Next(10, 100_001) / 100m;
        return new AliceSourceDto
        {
            Id = Guid.NewGuid().ToString(),
            Price = price.ToString(CultureInfo.InvariantCulture),
            Volume = (price * Random.Next(1, 1001)).ToString(CultureInfo.InvariantCulture)
        };
    }
}