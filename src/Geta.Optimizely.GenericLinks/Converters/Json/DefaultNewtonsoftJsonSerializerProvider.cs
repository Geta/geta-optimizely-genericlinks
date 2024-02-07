using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Geta.Optimizely.GenericLinks.Converters.Json;

public class DefaultNewtonsoftJsonSerializerProvider : INewtonsoftJsonSerializerProvider
{
    private readonly JsonSerializer _serializer;

    public DefaultNewtonsoftJsonSerializerProvider() 
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };

        _serializer = JsonSerializer.Create(settings);
    }

    public JsonSerializer GetSerializer()
    {
        return _serializer;
    }
}
