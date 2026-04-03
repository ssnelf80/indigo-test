using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace IndigoTestTask.DAL.Outbox;

public class OutboxConnectionFactory
{
    private readonly string _connectionString;
    
    public OutboxConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("OutboxConnection")!;
        if (string.IsNullOrEmpty(_connectionString))
            throw new NullReferenceException("Connection string is null or empty");
    }
    public IDbConnection Create() => new NpgsqlConnection(_connectionString);
}