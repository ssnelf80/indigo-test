using System.Text.Json;
using IndigoTestTask.Adapters.Sources;
using IndigoTestTask.Adapters.Sources.Clients.Adapters;
using IndigoTestTask.Adapters.Sources.Converters;
using IndigoTestTask.Adapters.Sources.Options;
using IndigoTestTask.Adapters.Sources.Servers.Handlers;
using IndigoTestTask.Adapters.SourceServers;
using IndigoTestTask.App.DatabusTickOutboxPublisher;
using IndigoTestTask.DAL.Outbox;
using IndigoTestTask.Domain.Repositories;
using KafkaFlow;
using KafkaFlow.Serializer;
using Polly;
using Polly.Retry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureOutboxFluentMigrator();

// todo {nazarov} через options
builder.Services.AddKafka(kafka =>
    kafka.AddCluster(cluster =>
        cluster.WithBrokers(["localhost:9092"])
            .AddProducer("outbox-producer", producer => producer.DefaultTopic(
                    "tick-queue")
                )
    ));

// Add services to the container.
builder.Services.AddSingleton<IDatabusPublisher, DatabusTickPublisher>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<IDatabusPublisher>());


builder.Services.AddSingleton<OutboxConnectionFactory>();
builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
builder.Services.AddSingleton<OutboxProvider>();

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

// todo {nazarov} по-идее можно создать ручками класс наследник и не резолвить через строки
builder.Services.AddResiliencePipeline("ws-client", builder =>
{
    builder.AddRetry(new RetryStrategyOptions
    {
        Name = null,
        MaxRetryAttempts = int.MaxValue,
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        Delay = TimeSpan.FromSeconds(1),
        MaxDelay = TimeSpan.FromSeconds(10),
        ShouldHandle = new PredicateBuilder().Handle<Exception>(),
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

app.MigrateOutboxDatabase();

// Configure the HTTP request pipeline.

app.UseWebSockets(new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromSeconds(2),
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();