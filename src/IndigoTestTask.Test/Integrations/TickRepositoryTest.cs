using IndigoTestTask.Domain.Entities;
using IndigoTestTask.Domain.Enums;
using IndigoTestTask.Domain.Repositories;
using Microsoft.Testing.Platform.Services;


namespace IndigoTestTask.Test.Integrations;

public class TickRepositoryTest(TickRepositoryServiceFixture tickRepositoryServiceFixture)
    : IClassFixture<TickRepositoryServiceFixture>
{
    private readonly ITickRepository _tickRepository = tickRepositoryServiceFixture.ServiceProvider.GetRequiredService<ITickRepository>();

    [Fact]
    public async Task TickRepository_ShouldIgnoreDuplicateMessage()
    {
        // Arrange
        var tick = new Tick
        {
            Ticker = Guid.NewGuid().ToString(),
            Timestamp = DateTimeOffset.UtcNow,
            Price = 10,
            Volume = 10,
            Stock = Stock.Alice
        };
        var tickSet = new HashSet<Tick>([tick]);
        // Act
        await _tickRepository.AddTicksAsync(tickSet, CancellationToken.None);
        await _tickRepository.AddTicksAsync(tickSet, CancellationToken.None);
        // Assert
        Assert.Equal(1, await _tickRepository.CountAsync(CancellationToken.None));
    }
}