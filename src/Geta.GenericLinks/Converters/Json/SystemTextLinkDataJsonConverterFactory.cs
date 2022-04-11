using EPiServer.Shell.Json;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Geta.GenericLinks.Converters.Json
{
    public class SystemTextLinkDataJsonConverterFactory : JsonConverterFactory, IJsonConverter
    {
        private readonly IServiceProvider _serviceProvider;

        public SystemTextLinkDataJsonConverterFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(LinkData).IsAssignableFrom(typeToConvert);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converterType = typeof(SystemTextLinkDataConverter<>).MakeGenericType(typeToConvert);

            return (JsonConverter)ActivatorUtilities.CreateInstance(_serviceProvider, converterType);
        }
    }
}
