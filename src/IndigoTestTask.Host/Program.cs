using IndigoTestTask.DAL;
using IndigoTestTask.Host.Extensions;
using KafkaFlow;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider((_, options) =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});

builder.AddSourceServers();
builder.AddOutbox();
builder.AddDatabus();
builder.AddDataSourceAdapters();
builder.AddCore();

builder.Services.AddControllers();

var app = builder.Build();
var bus = app.Services.CreateKafkaBus();
await bus.StartAsync();

app.Services.MigrateDatabases();

app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(2),
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

