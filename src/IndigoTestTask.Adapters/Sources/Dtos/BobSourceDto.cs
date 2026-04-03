using IndigoTestTask.Domain.Services.BaseTickConverter;

namespace IndigoTestTask.Adapters.Sources.Dtos;

public sealed record BobSourceDto(string Ticker, decimal TotalPrice, int Count) : ITickDto;