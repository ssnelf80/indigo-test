using Dapper;
using IndigoTestTask.Domain.Entities;
using IndigoTestTask.Domain.Repositories;

namespace IndigoTestTask.DAL.Ticks;

public class TickRepository(TickConnectionFactory connectionFactory) : ITickRepository
{
    public async Task AddTicksAsync(IReadOnlyCollection<Tick> ticks, CancellationToken cancellationToken)
    {
        const string query = """
                             INSERT INTO ticks (ticker, timestamp, price, volume, stock)
                                    SELECT v.ticker, v.timestamp, v.price, v.volume, v.stock
                                    FROM (
                                        SELECT 
                                            unnest(@Tickers) as ticker,
                                            unnest(@Timestamps) as timestamp,
                                            unnest(@Prices) as price,
                                            unnest(@Volumes) as volume,
                                            unnest(@Stocks)::stock_type as stock
                                    )
                                    WHERE NOT EXISTS (
                                        SELECT 1 FROM ticks t 
                                        WHERE t.hash = hash_record_extended(ROW(v.ticker, v.timestamp, v.price, v.volume, v.stock), 0)
                                          AND t.ticker = v.ticker 
                                          AND t.timestamp = v.timestamp 
                                          AND t.price = v.price 
                                          AND t.volume = v.volume 
                                          AND t.stock = v.stock
                                    );
                             """;
        
        using var connection = connectionFactory.Create();
        connection.Open();
        
        var parameters = new
        {
            Tickers = ticks.Select(t => t.Ticker).ToArray(),
            Timestamps = ticks.Select(t => t.Timestamp).ToArray(),
            Prices = ticks.Select(t => t.Price).ToArray(),
            Volumes = ticks.Select(t => t.Volume).ToArray(),
            Stocks = ticks.Select(t => t.Stock.ToString()).ToArray() 
        };
        await connection.ExecuteAsync(new CommandDefinition(query, parameters, cancellationToken: cancellationToken));
        connection.Close();
    }
}