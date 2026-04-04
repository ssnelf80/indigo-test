namespace IndigoTestTask.Adapters.Sources.Servers;

public class SourceServerOptions
{
    public static SourceServerOptions Instance => new SourceServerOptions
    {
        CanSendDuplicate = true,
        CanLongTimeAbort = true,
        CanSingleTimeAbort = true,
        Rps = 100
    };

    public bool CanSendDuplicate { get; init; }
    public bool CanLongTimeAbort { get; init; }
    public bool CanSingleTimeAbort { get; init; }
    public int Rps { get; init; }
}