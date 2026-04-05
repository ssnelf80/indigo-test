using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using IndigoTestTask.Adapters.Sources.Dtos;

namespace IndigoTestTask.Adapters.Sources.Options;

public static class TickDtoJsonSerializerOptions
{
    public static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };
}