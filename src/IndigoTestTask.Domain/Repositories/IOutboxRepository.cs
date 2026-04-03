namespace IndigoTestTask.Domain.Repositories;

public interface IOutboxRepository
{ 
    Task SaveAsync(byte[] message, CancellationToken cancellationToken);
}