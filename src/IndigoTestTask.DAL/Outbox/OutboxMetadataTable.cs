using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Options;

namespace IndigoTestTask.DAL.Outbox;

[VersionTableMetaData]
public class OutboxMetadataTable(IConventionSet conventionSet, IOptions<RunnerOptions> runnerOptions)
    : DefaultVersionTableMetaData(conventionSet, runnerOptions)
{
    public override string TableName => "OutboxVersionInfo";
    public override string UniqueIndexName => "UC_OutboxVersionInfo_Version";
}