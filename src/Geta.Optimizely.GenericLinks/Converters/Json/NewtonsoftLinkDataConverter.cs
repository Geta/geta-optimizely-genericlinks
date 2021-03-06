// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Geta.Optimizely.GenericLinks.Cms.EditorModels;
using Newtonsoft.Json;
using System;

namespace Geta.Optimizely.GenericLinks.Converters.Json
{
    public class NewtonsoftLinkDataConverter : JsonConverter
    {
        private readonly ILinkModelConverter _linkModelConverter;

        public NewtonsoftLinkDataConverter(ILinkModelConverter linkModelConverter)
        {
            _linkModelConverter = linkModelConverter;
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
            throw new NotSupportedException();
        }
    }
}
