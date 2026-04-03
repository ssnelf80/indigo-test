using IndigoTestTask.Adapters.Sources.Dtos;
using IndigoTestTask.Domain.Entities;
using IndigoTestTask.Domain.Enums;
using IndigoTestTask.Domain.Services.BaseTickConverter;

namespace IndigoTestTask.Adapters.Sources.Converters;

public class ChloeDomainTickConverter : IDomainTickConverter<ChloeSourceDto>
{
    public Tick ToDomainModel(ChloeSourceDto dto) =>
        new()
        {
            Ticker = dto.Ticker,
            Timestamp = dto.Timestamp,
            Price = dto.Price,
            Volume = dto.Volume,
            Stock = Stock.Chloe
        };
}