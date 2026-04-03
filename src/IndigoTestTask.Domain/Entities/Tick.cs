using IndigoTestTask.Domain.Enums;

namespace IndigoTestTask.Domain.Entities;

public record Tick
{
    public required string Ticker  { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required decimal Price { get; init; }
    public required decimal Volume { get; init; }
    public required Stock Stock { get; init; }
}

