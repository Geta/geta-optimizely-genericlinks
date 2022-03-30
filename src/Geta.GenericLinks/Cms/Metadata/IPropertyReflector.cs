using System;
using System.Collections.Generic;
using System.Reflection;

namespace Geta.GenericLinks.Cms.Metadata
{
    public interface IPropertyReflector
    {
        IReadOnlyCollection<PropertyInfo> GetProperties(Type type, bool inherited = true);
        object? GetValue(Type type, PropertyInfo property, object source);
    }
}
