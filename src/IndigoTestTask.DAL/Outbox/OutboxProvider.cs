using System.Data;
using Dapper;
using IndigoTestTask.DAL.Outbox.Entities;

namespace IndigoTestTask.DAL.Outbox;

public class OutboxProvider(OutboxConnectionFactory connectionFactory)
{
    private const string GetUnprocessedItemsQuery = """
                                                            SELECT id, message FROM outbox 
                                                            ORDER BY id
                                                            LIMIT 100        
                                                            FOR UPDATE SKIP LOCKED;
                                                    """;

    private const string RemoveProcessedItemsQuery = """
                                                  DELETE FROM outbox
                                                  WHERE id = ANY(@ProcessedItems);
                                                  """;

    
    public async Task<bool> ProcessAsync(Func<OutboxMessage[], CancellationToken, Task> action, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.Create();
        connection.Open();
        using var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
        
        try
        {
            var getUnprocessedItemsCommand = new CommandDefinition(
                commandText: GetUnprocessedItemsQuery,
                transaction: transaction,
                cancellationToken: cancellationToken
            );

            var messages = (await connection.QueryAsync<OutboxMessage>(getUnprocessedItemsCommand)).ToArray();
            
            if (messages.Length == 0)
                return false;
            
            await action(messages, cancellationToken);

            var setProcessedItemsCommand = new CommandDefinition(
                commandText: RemoveProcessedItemsQuery,
                parameters: new
                {
                    ProcessedItems =
                        messages.Select(x => x.Id).ToArray()
                },
                transaction: transaction,
                cancellationToken: cancellationToken
            );

            await connection.ExecuteAsync(setProcessedItemsCommand);

            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
       
        return true;
    }
}