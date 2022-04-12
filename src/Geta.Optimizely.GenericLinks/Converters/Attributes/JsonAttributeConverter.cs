// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Newtonsoft.Json;
using System;

namespace Geta.Optimizely.GenericLinks.Converters.Attributes
{
    public class JsonAttributeConverter : ILinkDataAttibuteConverter
    {
        public bool CanConvert(Type type)
        {
            if (type.IsAbstract)
                return false;

            if (type.IsInterface)
                return false;

            return typeof(object).IsAssignableFrom(type);
        }

        public string? Convert(object value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}
