using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace IndigoTestTask.DAL.Ticks;

public class TickConnectionFactory
{
    private readonly string _connectionString;
    
    public TickConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("TickConnection")!;
        if (string.IsNullOrEmpty(_connectionString))
            throw new NullReferenceException("Connection string is null or empty");
    }
    public IDbConnection Create() => new NpgsqlConnection(_connectionString);
}