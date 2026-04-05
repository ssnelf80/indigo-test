using System.Text.Json;
using IndigoTestTask.Domain.Entities;
using IndigoTestTask.Domain.Enums;
using IndigoTestTask.Domain.Repositories;
using KafkaFlow;
using Microsoft.Testing.Platform.Services;


namespace IndigoTestTask.Test.Integrations;

public class TickConnectionRepositoryTests(IntegrationTestServiceFixture integrationTestServiceFixture)
    : IClassFixture<IntegrationTestServiceFixture>
{
    private readonly ITickRepository _tickRepository = integrationTestServiceFixture.ServiceProvider.GetRequiredService<ITickRepository>();
    private readonly IDatabusPublisher _databusPublisher = integrationTestServiceFixture.ServiceProvider.GetRequiredService<IDatabusPublisher>();

    [Fact]
    public async Task TestFlow_ShouldIgnoreDuplicateMessage()
    {
        // Arrange
        await _databusPublisher.StartAsync(CancellationToken.None);
        var bus = integrationTestServiceFixture.ServiceProvider.CreateKafkaBus();
        await bus.StartAsync();
        
        var tick = new Tick
        {
            Ticker = Guid.NewGuid().ToString(),
            Timestamp = DateTimeOffset.UtcNow,
            Price = 10,
            Volume = 10,
            Stock = Stock.Alice
        };
        var message = JsonSerializer.SerializeToUtf8Bytes(tick);
       
        // Act
        await _databusPublisher.PublishAsync(message, CancellationToken.None);
        await _databusPublisher.PublishAsync(message, CancellationToken.None);
        await Task.Delay(10_000); // время на подключение к топику и обработку сообщений
        // Assert
        Assert.Equal(1, await _tickRepository.CountAsync(CancellationToken.None));
    }
}