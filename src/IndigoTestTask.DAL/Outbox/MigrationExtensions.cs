using System.Reflection;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IndigoTestTask.DAL.Outbox;

public static class MigrationExtensions
{
    public static void MigrateOutboxDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        migrationService.MigrateUp();
    }

    public static void ConfigureOutboxFluentMigrator(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddFluentMigratorCore()
            .ConfigureRunner(c => c.AddPostgres()
                .WithGlobalConnectionString(s => s.GetService<IConfiguration>()!.GetConnectionString("OutboxConnection"))
                .WithMigrationsIn(Assembly.GetAssembly(typeof(MigrationExtensions)))
            )
            .Configure<RunnerOptions>(options => options.Tags = ["Outbox"])
            .AddScoped<IVersionTableMetaData, OutboxMetadataTable>();
    }
}