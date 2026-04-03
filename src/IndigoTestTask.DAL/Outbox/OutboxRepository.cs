using Dapper;
using IndigoTestTask.Domain.Repositories;

namespace IndigoTestTask.DAL.Outbox;

public class OutboxRepository(OutboxConnectionFactory connectionFactory) : IOutboxRepository
{
    public async Task SaveAsync(byte[] message, CancellationToken cancellationToken)
    {
        const string query = "INSERT INTO outbox (message) VALUES (@Message);";
        using var connection = connectionFactory.Create();
        connection.Open();

        var command = new CommandDefinition(
            commandText: query,
            parameters: new
            {
                Message = message
            },
            cancellationToken: cancellationToken);
        
        await connection.ExecuteAsync(command);
        connection.Close();
    }
}