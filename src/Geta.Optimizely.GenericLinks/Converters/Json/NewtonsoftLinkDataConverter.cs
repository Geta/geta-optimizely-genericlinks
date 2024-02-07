// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Geta.Optimizely.GenericLinks.Cms.EditorModels;
using Geta.Optimizely.GenericLinks.Cms.Metadata;
using Geta.Optimizely.GenericLinks.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Geta.Optimizely.GenericLinks.Converters.Json;

public class NewtonsoftLinkDataConverter : JsonConverter
{
    private readonly IPropertyReflector _propertyReflector;        
    private readonly ILinkModelConverter _linkModelConverter;
    private readonly ISet<string> _propertyFilter;
    private readonly JsonSerializer _serializer;

    public NewtonsoftLinkDataConverter(
        IPropertyReflector propertyReflector,
        ILinkModelConverter linkModelConverter,
        INewtonsoftJsonSerializerProvider serializerProvider)
    {
        _propertyReflector = propertyReflector;
        _linkModelConverter = linkModelConverter;
        _serializer = serializerProvider.GetSerializer();
        _propertyFilter = new HashSet<string>();

        PopulatePropertyFilter();
    }

    public override bool CanConvert(Type objectType)
    {
        return typeof(ILinkData).IsAssignableFrom(objectType);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        LinkModel? clientModel;

        if (reader.TokenType == JsonToken.StartArray)
        {
            var array = serializer.Deserialize<LinkModel[]>(reader);
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
            clientModel = serializer.Deserialize<LinkModel>(reader);
        }

        if (clientModel is null)
            return null;

        return _linkModelConverter.ToServerModel(objectType, clientModel);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        var linkData = value as ILinkData;
        if (linkData is null)
            return;

        var linkDataType = linkData.GetType();
        var clientModel = _linkModelConverter.ToClientModel(linkData);
        var editorProperties = _propertyReflector.GetProperties(linkDataType, inherited: true);
        var propertyBuilder = new StringBuilder();
        using var propertyWriter = new StringWriter(propertyBuilder);

        writer.WriteStartObject();

        writer.WritePropertyName("text");
        writer.WriteValue(clientModel.Text);

        writer.WritePropertyName("href");
        writer.WriteValue(clientModel.Href);

        writer.WritePropertyName("title");
        writer.WriteValue(clientModel.Title);

        writer.WritePropertyName("target");

        if (clientModel.Target.HasValue)
        {
            writer.WriteValue(clientModel.Target.Value);
        }
        else
        {
            writer.WriteNull();
        }

        writer.WritePropertyName("typeIdentifier");
        writer.WriteValue(clientModel.TypeIdentifier);
        
        writer.WritePropertyName("publicUrl");
        writer.WriteValue(clientModel.PublicUrl);

        foreach (var property in editorProperties)
        {
            if (_propertyFilter.Contains(property.Name))
                continue;

            if (value is null)
                continue;

            var propertyValue = _propertyReflector.GetValue(linkDataType, property, value);
            if (propertyValue is null)
                continue;

            writer.WritePropertyName(property.Name.ToCamel());

            _serializer.Serialize(propertyWriter, propertyValue);
            
            writer.WriteRawValue(propertyBuilder.ToString());
            propertyBuilder.Clear();
        }

        writer.WritePropertyName("attributes");
        writer.WriteStartObject();

        foreach (var attribute in linkData.Attributes)
        {
            writer.WritePropertyName(attribute.Key);
            
            _serializer.Serialize(propertyWriter, attribute.Value);

            writer.WriteRawValue(propertyBuilder.ToString());
            propertyBuilder.Clear();
        }

        writer.WriteEndObject();
        writer.WriteEndObject();
    }

    private void PopulatePropertyFilter()
    {
        var properties = _propertyReflector.GetProperties(typeof(LinkData));
        foreach (var property in properties)
            _propertyFilter.Add(property.Name);
    }
}
