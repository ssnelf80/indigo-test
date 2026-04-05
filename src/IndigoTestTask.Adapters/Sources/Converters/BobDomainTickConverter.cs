using IndigoTestTask.Adapters.Sources.BaseTickConverter;
using IndigoTestTask.Adapters.Sources.Dtos;
using IndigoTestTask.Domain.Entities;
using IndigoTestTask.Domain.Enums;

namespace IndigoTestTask.Adapters.Sources.Converters;

public class BobDomainTickConverter : BaseDomainTickConverter<BobSourceDto>
{
    protected override Tick Convert(BobSourceDto dto) =>
        new()
        {
            Ticker = dto.Ticker,
            Timestamp = DateTimeOffset.UtcNow,
            Price = dto.TotalPrice,
            Volume = dto.Count * dto.TotalPrice,
            Stock = Stock.Bob
        };
}