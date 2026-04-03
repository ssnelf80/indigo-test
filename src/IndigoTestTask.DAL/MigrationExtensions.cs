using System.Reflection;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.VersionTableInfo;
using IndigoTestTask.DAL.Outbox;
using IndigoTestTask.DAL.Ticks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IndigoTestTask.DAL;

public static class MigrationExtensions
{
    public static void MigrateDatabases(this IHost host)
    {
        var outboxConnectionString = host.Services.GetService<IConfiguration>()!.GetConnectionString("OutboxConnection");
        var tickConnectionString = host.Services.GetService<IConfiguration>()!.GetConnectionString("TickConnection");
        MigrateOutbox(outboxConnectionString);
        MigrateTick(tickConnectionString);
    }

    private static void MigrateOutbox(string connectionString)
    {
        var serviceProvider = new ServiceCollection().AddFluentMigratorCore()
            .ConfigureRunner(c => c.AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .WithMigrationsIn(Assembly.GetAssembly(typeof(MigrationExtensions)))
            )
            .Configure<RunnerOptions>(options => options.Tags = ["Outbox"])
            .AddScoped<IVersionTableMetaData, OutboxMetadataTable>()
            .BuildServiceProvider(false);
        
        using var scope = serviceProvider.CreateScope();
        var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        migrationService.MigrateUp();
    }

    private static void MigrateTick(string connectionString)
    {
        var serviceProvider = new ServiceCollection().AddFluentMigratorCore()
            .ConfigureRunner(c => c.AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .WithMigrationsIn(Assembly.GetAssembly(typeof(MigrationExtensions)))
            )
            .Configure<RunnerOptions>(options => options.Tags = ["Ticks"])
            .AddScoped<IVersionTableMetaData, TickMetadataTable>()
            .BuildServiceProvider(false);
        
        using var scope = serviceProvider.CreateScope();
        var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        migrationService.MigrateUp();
    }
}