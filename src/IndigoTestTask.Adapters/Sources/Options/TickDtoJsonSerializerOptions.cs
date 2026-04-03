using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using IndigoTestTask.Adapters.Sources.Dtos;
using IndigoTestTask.Domain.Services.BaseTickConverter;

namespace IndigoTestTask.Adapters.Sources.Options;

public static class TickDtoJsonSerializerOptions
{
    public static JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions()
    {
        // WriteIndented = true,
        // TypeInfoResolver = new DefaultJsonTypeInfoResolver()
        // {
        //     Modifiers = { CreatePolymorphicModifier }
        // },
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };
        
    static void CreatePolymorphicModifier(JsonTypeInfo jsonTypeInfo)
    {
        if (jsonTypeInfo.Type == typeof(ITickDto))
        {
            jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "$type",
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor,
                DerivedTypes =
                {
                    new JsonDerivedType(typeof(AliceSourceDto), "alice"),
                    new JsonDerivedType(typeof(BobSourceDto), "bob"),
                    new JsonDerivedType(typeof(ChloeSourceDto), "chloe")
                }
            };
        }
    }
}