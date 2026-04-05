using IndigoTestTask.Domain.Entities;

namespace IndigoTestTask.Domain.Repositories;

public interface ITickRepository
{
    Task AddTicksAsync(IReadOnlySet<Tick> ticks, CancellationToken cancellationToken);
    Task<long> CountAsync(CancellationToken cancellationToken);
}