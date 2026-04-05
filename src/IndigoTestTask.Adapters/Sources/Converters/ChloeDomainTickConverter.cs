using IndigoTestTask.Adapters.Sources.BaseTickConverter;
using IndigoTestTask.Adapters.Sources.Dtos;
using IndigoTestTask.Domain.Entities;
using IndigoTestTask.Domain.Enums;

namespace IndigoTestTask.Adapters.Sources.Converters;

public class ChloeDomainTickConverter : BaseDomainTickConverter<ChloeSourceDto>
{
    protected override Tick Convert(ChloeSourceDto dto) =>
        new()
        {
            Ticker = dto.Ticker,
            Timestamp = dto.Timestamp,
            Price = dto.Price,
            Volume = dto.Volume,
            Stock = Stock.Chloe
        };
}