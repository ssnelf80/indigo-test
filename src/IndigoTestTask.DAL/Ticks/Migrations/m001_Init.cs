using FluentMigrator;

namespace IndigoTestTask.DAL.Ticks.Migrations;

[Tags("Ticks")]
[Migration(1)]
public class Init : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE TYPE stock_type AS ENUM ('alice', 'bob', 'chloe');

                    CREATE TABLE IF NOT EXISTS ticks (
                        ticker text NOT NULL,
                        timestamp timestamptz NOT NULL,
                        price numeric NOT NULL,
                        volume numeric NOT NULL,
                        stock stock_type NOT NULL,
                        hash bigint GENERATED ALWAYS AS (
                           hash_record_extended(ROW(ticker, timestamp, price, volume, stock), 0)
                        ) STORED
                    );

                    CREATE INDEX hash_index ON ticks (hash);
                    CREATE UNIQUE INDEX ticker_hash_index ON ticks (ticker, hash);
                    """);
    }

    public override void Down()
    {
        Execute.Sql("""
                        DROP TABLE ticks;
                        DROP TYPE stock_type;
                    """);
    }
}