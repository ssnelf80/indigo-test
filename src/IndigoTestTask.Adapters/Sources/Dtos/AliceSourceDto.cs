using IndigoTestTask.Adapters.Sources.BaseTickConverter;

namespace IndigoTestTask.Adapters.Sources.Dtos;

public sealed record AliceSourceDto : ITickDto
{
    public required string Id { get; init; }
    public required string Price { get; init; }
    public required string Volume { get; init; }
}