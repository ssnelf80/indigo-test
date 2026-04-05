using IndigoTestTask.Adapters.Sources.BaseTickConverter;

namespace IndigoTestTask.Adapters.Sources.Dtos;

public sealed record BobSourceDto : ITickDto
{
    public required string Ticker { get; init; }
    public required decimal TotalPrice { get; init; }
    public required int Count { get; init; }
}