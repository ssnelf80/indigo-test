using IndigoTestTask.Domain.Entities;

namespace IndigoTestTask.Domain.Services.BaseTickConverter;

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
            throw new DomainConverterException(ex.Message);
        }
    }
    
    protected abstract Tick Convert(T value);
}