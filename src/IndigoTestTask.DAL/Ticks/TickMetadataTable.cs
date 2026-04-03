using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Options;

namespace IndigoTestTask.DAL.Ticks;

[VersionTableMetaData]
public class TickMetadataTable(IConventionSet conventionSet, IOptions<RunnerOptions> runnerOptions)
    : DefaultVersionTableMetaData(conventionSet, runnerOptions)
{
    public override string TableName => "TickVersionInfo";
    public override string UniqueIndexName => "UC_TickVersionInfo_Version";
}