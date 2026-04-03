using FluentMigrator;

namespace IndigoTestTask.DAL.Outbox.Migrations;

[Tags("Outbox")]
[Migration(1)]
public class Init : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                            CREATE TABLE IF NOT EXISTS outbox
                            (
                                id              bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
                                message         bytea NOT NULL,
                                created_at      timestamptz NOT NULL DEFAULT now()              
                            );     
                    """);
        
    }

    public override void Down()
    {
        Execute.Sql("""
                    DROP TABLE IF EXISTS outbox
                    """);
    }
}