namespace IndigoTestTask.Adapters.Sources.Servers;

public abstract class BaseSourceServerOptions
{
    public bool CanSendDuplicate { get; init; }
    public bool CanLongTimeAbort { get; init; }
    public bool CanSingleTimeAbort { get; init; }
    public int Rps { get; init; }
}