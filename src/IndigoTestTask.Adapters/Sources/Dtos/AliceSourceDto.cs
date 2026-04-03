using IndigoTestTask.Domain.Services.BaseTickConverter;

namespace IndigoTestTask.Adapters.Sources.Dtos;

public sealed record AliceSourceDto(string Id, string Price, string Volume) : ITickDto;