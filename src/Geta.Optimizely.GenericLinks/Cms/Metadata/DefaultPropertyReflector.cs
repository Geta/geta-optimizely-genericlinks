// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Geta.Optimizely.GenericLinks.Cms.Metadata
{
    public class DefaultPropertyReflector : IPropertyReflector
    {
        private readonly IDictionary<Type, IReadOnlyCollection<PropertyInfo>> _typeProperties;
        private readonly IDictionary<Type, IReadOnlyCollection<PropertyInfo>> _declaredTypeProperties;
        private readonly IDictionary<Type, IDictionary<PropertyInfo, Func<object, object>>> _typeAccessors;

        public DefaultPropertyReflector()
        {
            _typeProperties = new Dictionary<Type, IReadOnlyCollection<PropertyInfo>>();
            _declaredTypeProperties = new Dictionary<Type, IReadOnlyCollection<PropertyInfo>>();
            _typeAccessors = new Dictionary<Type, IDictionary<PropertyInfo, Func<object, object>>>();
        }

        public virtual IReadOnlyCollection<PropertyInfo> GetProperties(Type type, bool inherited = true)
        {
            var typeProperties = inherited ? _typeProperties : _declaredTypeProperties;

            if (typeProperties.TryGetValue(type, out var properties))
                return properties;

            properties = ReflectProperties(type, inherited);

            typeProperties.TryAdd(type, properties);

            return properties;
        }

        public virtual object? GetValue(Type type, PropertyInfo property, object source)
        {
            var accessors = GetAccessors(type);

            if (accessors.TryGetValue(property, out var accessor))
                return accessor(source);

            throw new InvalidOperationException($"Could not find a get accessor for '{type.Name}.{property.Name}'");
        }

        protected virtual IDictionary<PropertyInfo, Func<object, object>> GetAccessors(Type type)
        {
            if (_typeAccessors.TryGetValue(type, out var accessors))
                return accessors;

            accessors = new Dictionary<PropertyInfo, Func<object, object>>();

            var properties = GetProperties(type);

            foreach (var property in properties)
            {
                var method = property.GetGetMethod();
                if (method is null)
                    continue;

                var accessor = BuildGetAccessor(type, method);
                accessors.Add(property, accessor);
            }

            _typeAccessors.TryAdd(type, accessors);
            return accessors;
        }

        protected virtual IReadOnlyCollection<PropertyInfo> ReflectProperties(Type type, bool inherited)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty;

            if (!inherited)
                flags |= BindingFlags.DeclaredOnly;

            return type.GetProperties(flags);
        }

        protected virtual Func<object, object> BuildGetAccessor(Type type, MethodInfo method)
        {
            var parameterIn = Expression.Parameter(typeof(object), "o");
            var conversionIn = Expression.Convert(parameterIn, type);
            var invocation = Expression.Call(conversionIn, method);
            var conversionOut = Expression.Convert(invocation, typeof(object));
            var accessor = Expression.Lambda<Func<object, object>>(conversionOut, parameterIn);

            return accessor.Compile();
        }
    }
}
