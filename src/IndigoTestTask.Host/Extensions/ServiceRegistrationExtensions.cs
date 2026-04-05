using IndigoTestTask.Adapters.Sources.Clients.Adapters;
using IndigoTestTask.Adapters.Sources.Converters;
using IndigoTestTask.Adapters.Sources.Options;
using IndigoTestTask.Adapters.Sources.Servers.Handlers;
using IndigoTestTask.App;
using IndigoTestTask.App.DatabusTickConsumer;
using IndigoTestTask.App.DatabusTickOutboxPublisher;
using IndigoTestTask.DAL.Outbox;
using IndigoTestTask.DAL.Ticks;
using IndigoTestTask.Domain.Entities;
using IndigoTestTask.Domain.Repositories;
using KafkaFlow;
using KafkaFlow.Retry;
using KafkaFlow.Serializer;
using Polly;
using Polly.Retry;

namespace IndigoTestTask.Host.Extensions;

public static class ServiceRegistrationExtensions
{
    public static void AddSourceServers(this IHostApplicationBuilder builder)
    {
        // server options
        builder.Services.Configure<AliceServiceOptions>(
            builder.Configuration.GetSection("AliceServiceOptions"));
        builder.Services.Configure<BobServiceOptions>(
            builder.Configuration.GetSection("BobServiceOptions"));
        builder.Services.Configure<ChloeServiceOptions>(
            builder.Configuration.GetSection("ChloeServiceOptions"));
        
        // server handlers
        builder.Services.AddSingleton<AliceSourceServerHandler>();
        builder.Services.AddSingleton<BobSourceServerHandler>();
        builder.Services.AddSingleton<ChloeSourceServerHandler>();
    }
    public static void AddOutbox(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<OutboxConnectionFactory>();
        builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
        builder.Services.AddSingleton<OutboxProvider>();
    }
    public static void AddDatabus(this IHostApplicationBuilder builder)
    {
        builder.Services.AddKafka(kafka =>
            kafka
                .UseMicrosoftLog()
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
        
        builder.Services.AddSingleton<IDatabusPublisher, DatabusTickPublisher>();
        builder.Services.AddHostedService(sp => sp.GetRequiredService<IDatabusPublisher>());
    }
    public static void AddDataSourceAdapters(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<AliceAdapterOptions>(
            builder.Configuration.GetSection("AliceAdapterOptions"));
        builder.Services.Configure<BobAdapterOptions>(
            builder.Configuration.GetSection("BobAdapterOptions"));
        builder.Services.Configure<ChloeAdapterOptions>(
            builder.Configuration.GetSection("ChloeAdapterOptions"));

        builder.Services.AddSingleton<AliceDomainTickConverter>();
        builder.Services.AddSingleton<BobDomainTickConverter>();
        builder.Services.AddSingleton<ChloeDomainTickConverter>();

        builder.Services.AddHostedService<AliceDataSourceAdapter>();
        builder.Services.AddHostedService<BobDataSourceAdapter>();
        builder.Services.AddHostedService<ChloeDataSourceAdapter>();
        
        builder.Services.AddResiliencePipeline("ws-client", (builder) =>
        {
            builder.AddRetry(new RetryStrategyOptions
            {
                Name = null,
                MaxRetryAttempts = int.MaxValue,
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                Delay = TimeSpan.FromSeconds(1),
                MaxDelay = TimeSpan.FromSeconds(10),
                ShouldHandle = new PredicateBuilder().Handle<Exception>()
            });
        });
    }
    public static void AddCore(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<TickConnectionFactory>();
        builder.Services.AddScoped<ITickRepository, TickConnectionRepository>();

        builder.Services.AddHostedService<TickProcessedChecker>();
    }
}