// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Geta.Optimizely.GenericLinks;

public abstract class PropertyLinkBase : PropertyLongString
{
    private static readonly FieldInfo? LazyFactoryField =
        typeof(PropertyLongString).GetField("_lazyValueFactory", BindingFlags.NonPublic | BindingFlags.Instance);

    protected PropertyLinkBase()
    {
    }

    protected PropertyLinkBase(string value) : base(value)
    {
    }

    // CMS 13 PropertyLongString.LongString getter casts the lazy value to (string).
    // When the lazy factory returns a typed object instead of a string, that cast fails.
    // This method consumes the factory directly, bypassing the base class's string-only path.
    protected object? ConsumeLazyValue()
    {
        if (LazyFactoryField == null) return null;
        var factory = LazyFactoryField.GetValue(this) as Func<object>;
        if (factory == null) return null;
        LazyFactoryField.SetValue(this, null);
        return factory();
    }

    protected bool HasBaseLazyValue()
    {
        if (LazyFactoryField == null) return false;
        return LazyFactoryField.GetValue(this) != null;
    }

    public override object? SaveData()
    {
        return ToLongString();
    }

    [Obsolete]
    public override object? SaveData(PropertyDataCollection properties)
    {
        return ToLongString();
    }

    protected abstract string? ToLongString();

    protected abstract string SanitizeValue(string value);

    protected abstract string GetPermanentUrl(string href);

    protected virtual void ParseAttributes(XElement element, ILinkData linkItem)
    {
        var attributes = GetDefinedAttributes(element);

        foreach (var attribute in attributes)
        {
            linkItem.Attributes.Add(attribute.Key, attribute.Value);
        }

        linkItem.SetAttributes(attributes);
    }

    protected virtual XElement CreateLinkElement(ILinkData linkItem)
    {
        var attributes = linkItem.GetAttributes();
        var element = new XElement("a", CreateElementAttributes(attributes));

        if (!string.IsNullOrWhiteSpace(linkItem.Href))
            element.SetAttributeValue("href", GetPermanentUrl(linkItem.Href));

        if (!string.IsNullOrWhiteSpace(linkItem.Text))
            element.SetValue(SanitizeValue(linkItem.Text));

        return element;
    }

    protected virtual IEnumerable<XAttribute> CreateElementAttributes(IDictionary<string, string> attributes)
    {
        foreach (var attribute in attributes)
        {
            var value = SanitizeValue(attribute.Value);
            if (string.IsNullOrEmpty(value))
                continue;

            yield return new XAttribute(attribute.Key, value);
        }
    }

    protected virtual XElement GetLinkElement(string value)
    {
        return GetLinkElements(value).First();
    }

    protected virtual IEnumerable<XElement> GetLinkElements(string value)
    {
        var rootElement = XElement.Parse(value);

        foreach (var element in rootElement.Descendants())
        {
            if (element.Name != "a")
                continue;

            yield return element;
        }
    }

    protected virtual IDictionary<string, string> GetDefinedAttributes(XElement element)
    {
        var attributes = element.Attributes();
        var attributeLookup = new Dictionary<string, string>();

        foreach (var attribute in attributes)
        {
            if (string.IsNullOrEmpty(attribute.Value))
                continue;

            var name = attribute.Name.LocalName;
            if (attributeLookup.ContainsKey(name))
                continue;

            attributeLookup.Add(name, attribute.Value);
        }

        return attributeLookup;
    }

}
