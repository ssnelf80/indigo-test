using IndigoTestTask.Adapters.Sources.Dtos;
using IndigoTestTask.Domain.Entities;
using IndigoTestTask.Domain.Enums;
using IndigoTestTask.Domain.Services.BaseTickConverter;

namespace IndigoTestTask.Adapters.Sources.Converters;

public class BobDomainTickConverter : IDomainTickConverter<BobSourceDto>
{
    public Tick ToDomainModel(BobSourceDto dto) =>
        new()
        {
            Ticker = dto.Ticker,
            Timestamp = DateTimeOffset.UtcNow,
            Price = dto.TotalPrice,
            Volume = dto.Count * dto.TotalPrice,
            Stock = Stock.Bob
        };
}