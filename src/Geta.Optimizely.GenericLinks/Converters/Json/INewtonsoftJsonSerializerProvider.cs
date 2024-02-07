using Newtonsoft.Json;

namespace Geta.Optimizely.GenericLinks.Converters.Json;

public interface INewtonsoftJsonSerializerProvider
{
    JsonSerializer GetSerializer();
}
