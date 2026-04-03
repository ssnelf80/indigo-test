using System.Globalization;
using IndigoTestTask.Adapters.Sources.Dtos;
using IndigoTestTask.Adapters.SourceServers;

namespace IndigoTestTask.Adapters.Sources.Servers.Handlers;

public sealed class AliceSourceServerHandler(SourceServerOptions options)
    : BaseSourceServerHandler<AliceSourceDto>(options)
{
    protected override AliceSourceDto GenerateMessage()
    {
        var price = Random.Next(10, 100_001) / 100m;
        return new AliceSourceDto(
            Guid.NewGuid().ToString(),
            price.ToString(CultureInfo.InvariantCulture),
            (price * Random.Next(1, 1001)).ToString(CultureInfo.InvariantCulture));
    }
}