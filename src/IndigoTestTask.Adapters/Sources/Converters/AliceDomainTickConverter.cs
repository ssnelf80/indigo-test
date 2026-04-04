using System.Globalization;
using IndigoTestTask.Adapters.Sources.Dtos;
using IndigoTestTask.Domain.Entities;
using IndigoTestTask.Domain.Enums;
using IndigoTestTask.Domain.Services.BaseTickConverter;

namespace IndigoTestTask.Adapters.Sources.Converters;

public class AliceDomainTickConverter : BaseDomainTickConverter<AliceSourceDto>
{
    protected override Tick Convert(AliceSourceDto dto) =>
        new()
        {
            Ticker = dto.Id,
            Timestamp = DateTimeOffset.UtcNow,
            Price = decimal.Parse(dto.Price, CultureInfo.InvariantCulture),
            Volume = decimal.Parse(dto.Volume, CultureInfo.InvariantCulture),
            Stock = Stock.Alice
        };
}

