using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using IndigoTestTask.App.DatabusTickConsumer;
using IndigoTestTask.App.DatabusTickOutboxPublisher;
using IndigoTestTask.DAL;
using IndigoTestTask.DAL.Outbox;
using IndigoTestTask.DAL.Ticks;
using IndigoTestTask.Domain.Entities;
using IndigoTestTask.Domain.Repositories;
using KafkaFlow;
using KafkaFlow.Retry;
using KafkaFlow.Serializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IndigoTestTask.Test.Integrations;

public class IntegrationTestServiceFixture : IDisposable
{
    private const string Dockerfile = "docker-compose.test.yml";
    public ICompositeService CompositeService { get; }
    public IServiceProvider ServiceProvider { get; }

    public IntegrationTestServiceFixture()
    {
        var composeFile = FindRootPath() + '\\' + Dockerfile;
        
        CompositeService = new Builder()
            .UseContainer()
            .UseCompose()
            .FromFile(composeFile)
            .RemoveOrphans()
            .WaitForHealthy(1_000)
            .Build()
            .Start();
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();
        
        var services = new ServiceCollection();
        
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<TickConnectionFactory>();
        services.AddScoped<ITickRepository, TickConnectionRepository>();
        
        services.AddSingleton<OutboxConnectionFactory>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddSingleton<OutboxProvider>();
        
        services.AddKafka(kafka =>
            kafka
                .AddCluster(cluster =>
                    cluster.WithBrokers(["localhost:9092"])
                        .CreateTopicIfNotExists("tick-queue", 1)
                        .AddProducer("outbox-producer", producer => producer.DefaultTopic(
                            "tick-queue")
                        )
                        .AddConsumer(consumer => consumer
                            .Topic("tick-queue")
                            .WithGroupId("tick-consumers")
                            .WithBufferSize(100)
                            .WithWorkersCount(4)
                            .WithAutoOffsetReset(AutoOffsetReset.Earliest)
                            .WithAutoCommitIntervalMs(3_000) 
                            .AddMiddlewares(middlewares =>
                                middlewares
                                    .AddSingleTypeDeserializer<Tick, JsonCoreDeserializer>()
                                    .RetryForever(retry => retry.WithTimeBetweenTriesPlan(TimeSpan.FromSeconds(1)))
                                    .AddBatching(100, TimeSpan.FromSeconds(1))
                                    .Add<DatabusTickBatchConsumerHandler>()
                            )
                        )));
        
        services.AddSingleton<IDatabusPublisher, DatabusTickPublisher>();
        
        ServiceProvider = services.BuildServiceProvider();
        ServiceProvider.MigrateDatabases();
    }

    private string FindRootPath()
    {
        var currentDir = new DirectoryInfo(AppContext.BaseDirectory);
        while (currentDir != null)
        {
            if (currentDir.GetFiles(Dockerfile).Any())
                return currentDir.FullName;
            currentDir = currentDir.Parent;
        }
        throw new DirectoryNotFoundException($"{Dockerfile} not found");
    }

    public void Dispose()
    {
        CompositeService.Dispose();
    }
}