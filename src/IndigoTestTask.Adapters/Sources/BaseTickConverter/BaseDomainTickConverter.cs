using IndigoTestTask.Domain.Entities;

namespace IndigoTestTask.Adapters.Sources.BaseTickConverter;

public abstract class BaseDomainTickConverter<T> where T : ITickDto
{
    public Tick ToDomainModel(T source)
    {
        try
        {
            return Convert(source);
        }
        catch (Exception ex)
        {
            throw new DomainConverterException($"Failed convert dto to domain model: {ex.Message}");
        }
    }
    
    protected abstract Tick Convert(T value);
}