using IndigoTestTask.Domain.Entities;

namespace IndigoTestTask.Adapters.Sources.BaseTickConverter;

public abstract class BaseDomainTickConverter<T> where T : ITickDto
{
    public Tick ToDomainModel(T source)
    {
        Tick tick;
        try
        {
            tick = Convert(source);
        }
        catch (Exception ex)
        {
            throw new DomainConverterException($"Failed convert dto to domain model: {ex.Message}");
        }
        
        if (!Validate(tick))
            throw new DomainConverterException($"Invalid tick model");

        return tick;
    }

    protected virtual bool Validate(Tick tick)
    {
        if (string.IsNullOrWhiteSpace(tick.Ticker) || tick.Timestamp == default)
            return false;
        
        return true;
    }
    
    protected abstract Tick Convert(T value);
}