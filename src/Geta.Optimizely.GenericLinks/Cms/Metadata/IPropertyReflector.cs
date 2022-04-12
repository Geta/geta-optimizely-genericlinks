// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Geta.Optimizely.GenericLinks.Cms.Metadata
{
    public interface IPropertyReflector
    {
        IReadOnlyCollection<PropertyInfo> GetProperties(Type type, bool inherited = true);
        object? GetValue(Type type, PropertyInfo property, object source);
    }
}
