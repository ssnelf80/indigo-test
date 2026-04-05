using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using IndigoTestTask.DAL;
using IndigoTestTask.DAL.Ticks;
using IndigoTestTask.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IndigoTestTask.Test.Integrations;

public class TickRepositoryServiceFixture : IDisposable
{
    private const string Dockerfile = "docker-compose.test.yml";
    public ICompositeService CompositeService { get; }
    public IServiceProvider ServiceProvider { get; }

    public TickRepositoryServiceFixture()
    {
        var composeFile = FindRootPath() + '\\' + Dockerfile;
        
        CompositeService = new Builder()
            .UseContainer()
            .UseCompose()
            .FromFile(composeFile)
            .RemoveOrphans() 
            .Build()
            .Start();
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();
        
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<TickConnectionFactory>();
        services.AddScoped<ITickRepository, TickRepository>();
        
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