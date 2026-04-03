using IndigoTestTask.Domain.Entities;

namespace IndigoTestTask.Domain.Services.BaseTickConverter;

public interface IDomainTickConverter<T> where T : ITickDto
{
    Tick ToDomainModel(T value);
}