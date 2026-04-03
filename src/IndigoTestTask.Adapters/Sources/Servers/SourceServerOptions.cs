namespace IndigoTestTask.Adapters.SourceServers;

public class SourceServerOptions
{
    public static SourceServerOptions Instance => new SourceServerOptions
    {
        CanSendDuplicate = false,
        CanLongTimeAbort = false,
        CanSingleTimeAbort = false,
        Rps = 100
    };

    public bool CanSendDuplicate { get; init; }
    public bool CanLongTimeAbort { get; init; }
    public bool CanSingleTimeAbort { get; init; }
    public int Rps { get; init; }
}