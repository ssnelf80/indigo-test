namespace IndigoTestTask.Adapters.Sources.Servers;

public abstract class BaseSourceServerOptions
{
    public int SendDuplicateIntervalSec { get; init; }
    public int UnavailableIntervalSec { get; init; }
    public int SingleTimeAbortIntervalSec { get; init; }
    public int SendInvalidMessageIntervalSec { get; init; }
    public int Rps { get; init; }
}