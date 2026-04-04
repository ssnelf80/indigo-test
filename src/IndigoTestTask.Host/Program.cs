using IndigoTestTask.Adapters.Sources.Clients.Adapters;
using IndigoTestTask.Adapters.Sources.Converters;
using IndigoTestTask.Adapters.Sources.Options;
using IndigoTestTask.Adapters.Sources.Servers;
using IndigoTestTask.Adapters.Sources.Servers.Handlers;
using IndigoTestTask.App;
using IndigoTestTask.App.DatabusTickConsumer;
using IndigoTestTask.App.DatabusTickOutboxPublisher;
using IndigoTestTask.DAL;
using IndigoTestTask.DAL.Outbox;
using IndigoTestTask.DAL.Ticks;
using IndigoTestTask.Domain.Entities;
using IndigoTestTask.Domain.Repositories;
using KafkaFlow;
using KafkaFlow.Serializer;
using Polly;
using Polly.Retry;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider((_, options) =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});


// Add services to the container.
builder.Services.AddSingleton<IDatabusPublisher, DatabusTickPublisher>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<IDatabusPublisher>());


builder.Services.AddSingleton<OutboxConnectionFactory>();
builder.Services.AddSingleton<TickConnectionFactory>();
builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
builder.Services.AddSingleton<OutboxProvider>();
builder.Services.AddScoped<ITickRepository, TickRepository>();

// todo {nazarov} вынести в опции
builder.Services.AddSingleton<AliceAdapterOptions>();
builder.Services.AddSingleton<BobAdapterOptions>();
builder.Services.AddSingleton<ChloeAdapterOptions>();

builder.Services.AddSingleton<AliceDomainTickConverter>();
builder.Services.AddSingleton<BobDomainTickConverter>();
builder.Services.AddSingleton<ChloeDomainTickConverter>();

builder.Services.AddHostedService<AliceDataSourceAdapter>();
builder.Services.AddHostedService<BobDataSourceAdapter>();
builder.Services.AddHostedService<ChloeDataSourceAdapter>();

builder.Services.AddHostedService<TickProcessedChecker>();

builder.Services.AddKafka(kafka =>
    kafka
        .UseMicrosoftLog()
        .AddCluster(cluster =>
        cluster.WithBrokers(["localhost:9092"])
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
                        .AddBatching(100, TimeSpan.FromSeconds(1))
                        .Add<DatabusTickBatchConsumerHandler>()
                )
            )));

// todo {nazarov} по-идее можно создать ручками класс наследник и не резолвить через строки
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

builder.Services.AddControllers();

builder.Services.AddSingleton<AliceSourceServerHandler>(sp =>
    new AliceSourceServerHandler(SourceServerOptions.Instance));

builder.Services.AddSingleton<BobSourceServerHandler>(sp =>
    new BobSourceServerHandler(SourceServerOptions.Instance));

builder.Services.AddSingleton<ChloeSourceServerHandler>(sp =>
    new ChloeSourceServerHandler(SourceServerOptions.Instance));



var app = builder.Build();
var bus = app.Services.CreateKafkaBus();
await bus.StartAsync();


app.MigrateDatabases();



// Configure the HTTP request pipeline.

app.UseWebSockets(new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromSeconds(2),
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();



app.Run();

