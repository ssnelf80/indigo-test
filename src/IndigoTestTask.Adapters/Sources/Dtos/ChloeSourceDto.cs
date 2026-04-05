using IndigoTestTask.Adapters.Sources.BaseTickConverter;

namespace IndigoTestTask.Adapters.Sources.Dtos;

public sealed record ChloeSourceDto : ITickDto
{
    public required string Ticker { get; init; }
    public required decimal Price { get; init; }
    public required decimal Volume { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
}