using IndigoTestTask.Domain.Services.BaseTickConverter;

namespace IndigoTestTask.Adapters.Sources.Dtos;

public sealed record ChloeSourceDto(string Ticker, decimal Price, decimal Volume, DateTimeOffset Timestamp) : ITickDto;