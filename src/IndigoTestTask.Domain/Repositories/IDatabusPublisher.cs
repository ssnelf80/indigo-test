using Microsoft.Extensions.Hosting;

namespace IndigoTestTask.Domain.Repositories;

public interface IDatabusPublisher : IHostedService
{
    Task PublishAsync(byte[] message, CancellationToken cancellationToken);
}