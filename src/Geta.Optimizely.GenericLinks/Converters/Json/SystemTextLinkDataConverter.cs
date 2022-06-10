// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Geta.Optimizely.GenericLinks.Cms.EditorModels;
using Geta.Optimizely.GenericLinks.Cms.Metadata;
using Geta.Optimizely.GenericLinks.Converters.Values;
using Geta.Optimizely.GenericLinks.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Geta.Optimizely.GenericLinks.Converters.Json
{
    public class SystemTextLinkDataConverter<TLinkData> : JsonConverter<TLinkData>
        where TLinkData : LinkData, new()
    {
        private readonly IPropertyReflector _propertyReflector;
        private readonly ILinkModelConverter _linkModelConverter;
        private readonly IEnumerable<ILinkDataValueWriter> _valueWriters;
        private readonly Dictionary<Type, ILinkDataValueWriter> _resolvedValueWriters;
        private readonly ISet<string> _propertyFilter;

        public SystemTextLinkDataConverter(
            IPropertyReflector propertyReflector,
            ILinkModelConverter linkModelConverter,
            IEnumerable<ILinkDataValueWriter> valueWriters)
        {
            _propertyReflector = propertyReflector;
            _linkModelConverter = linkModelConverter;
            _valueWriters = valueWriters;
            _resolvedValueWriters = new Dictionary<Type, ILinkDataValueWriter>();
            _propertyFilter = new HashSet<string>();

            PopulatePropertyFilter();
        }

        public override bool CanConvert(Type typeToConvert) => typeof(TLinkData).IsAssignableFrom(typeToConvert);

        public override TLinkData? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            LinkModel? clientModel;

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                TryDeserialize(ref reader, options, out LinkModel[]? array);
                if (array is not null && array.Length > 0)
                {
                    clientModel = array[0];
                }
                else
                {
                    clientModel = null;
                }
            }
            else
            {
                TryDeserialize(ref reader, options, out clientModel);
            }
            
            if (clientModel is null)
                return null;

            clientModel.Attributes = JsonElementsToString(clientModel.Attributes);

            return _linkModelConverter.ToServerModel<TLinkData>(clientModel);
        }

        protected virtual bool TryDeserialize<T>(ref Utf8JsonReader reader, JsonSerializerOptions options, out T? result)
            where T : class
        {
            try
            {
                result = JsonSerializer.Deserialize<T>(ref reader, options);
                return true;
            }
            catch (JsonException ex) when (ex.Path == "$")
            {
                result = null;
                return false;
            }
        }

        protected virtual Dictionary<string, object?> JsonElementsToString(Dictionary<string, object?> jsonAttributes)
        {
            var attributes = new Dictionary<string, object?>(jsonAttributes.Count);

            foreach (var key in jsonAttributes.Keys)
            {
                var value = jsonAttributes[key];

                if (value is JsonElement jsonElement)
                    value = GetValue(jsonElement);

                if (value is null)
                    continue;

                attributes.Add(key, value);
            }

            return attributes;
        }

        protected virtual object? GetValue(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Null: return null;
                case JsonValueKind.Undefined: return null;
                case JsonValueKind.String: return element.GetString();
                case JsonValueKind.Object:
                case JsonValueKind.Array:
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False:
                default: return element.GetRawText();
            }
        }

        protected virtual object GetNumber(JsonElement element)
        {
            var text = element.GetRawText();
            if (text.IndexOf('.') > -1)
            {
                return Convert.ToDouble(text);
            }
            else
            {
                return Convert.ToInt32(text);
            }
        }

        public override void Write(Utf8JsonWriter writer, TLinkData value, JsonSerializerOptions options)
        {
            var clientModel = _linkModelConverter.ToClientModel(value);
            var editorProperties = _propertyReflector.GetProperties(typeof(TLinkData));

            writer.WriteStartObject();
            writer.WriteString("text", clientModel.Text);
            writer.WriteString("href", clientModel.Href);
            writer.WriteString("title", clientModel.Title);

            if (clientModel.Target.HasValue)
            {
                writer.WriteNumber("target", clientModel.Target.Value);
            }
            else
            {
                writer.WriteNull("target");
            }

            writer.WriteString("typeIdentifier", clientModel.TypeIdentifier);
            writer.WriteString("publicUrl", clientModel.PublicUrl);

            foreach (var property in editorProperties)
            {
                if (_propertyFilter.Contains(property.Name))
                    continue;

                var propertyValue = _propertyReflector.GetValue(typeof(TLinkData), property, value);
                if (propertyValue is null)
                    continue;

                var valueWriter = FindValueWriter(property);
                if (valueWriter is null)
                    continue;

                writer.WritePropertyName(property.Name.ToCamel());
                valueWriter.Write(writer, propertyValue);
            }

            writer.WritePropertyName("attributes");
            writer.WriteStartObject();

            foreach (var attribute in value.Attributes)
            {
                writer.WriteString(attribute.Key, attribute.Value);
            }

            writer.WriteEndObject();
            writer.WriteEndObject();
        }

        protected virtual ILinkDataValueWriter? FindValueWriter(PropertyInfo property)
        {
            var propertyType = property.PropertyType;

            if (_resolvedValueWriters.TryGetValue(propertyType, out var converter))
                return converter;

            foreach (var valueWriter in _valueWriters)
            {
                if (!valueWriter.CanWrite(propertyType))
                    continue;

                _resolvedValueWriters.TryAdd(propertyType, valueWriter);
                return valueWriter;
            }

            return null;
        }

        private void PopulatePropertyFilter()
        {
            var properties = _propertyReflector.GetProperties(typeof(LinkData));
            foreach (var property in properties)
                _propertyFilter.Add(property.Name);
        }
    }
}
