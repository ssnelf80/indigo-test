using IndigoTestTask.Domain.Entities;

namespace IndigoTestTask.Domain.Repositories;

public interface ITickRepository
{
    Task AddTicksAsync(IReadOnlyCollection<Tick> ticks, CancellationToken cancellationToken);
    Task<long> CountAsync(CancellationToken cancellationToken);
}