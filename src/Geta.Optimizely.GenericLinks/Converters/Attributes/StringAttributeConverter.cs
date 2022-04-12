// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace Geta.Optimizely.GenericLinks.Converters.Attributes
{
    public class StringAttributeConverter : ILinkDataAttibuteConverter
    {
        public bool CanConvert(Type type)
        {
            return typeof(string).IsAssignableFrom(type);
        }

        public string? Convert(object value)
        {
            return (string)value;
        }
    }
}
