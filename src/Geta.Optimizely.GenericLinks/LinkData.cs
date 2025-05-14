// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.DataAnnotations;
using EPiServer.Data.Entity;
using EPiServer.Web;
using Geta.Optimizely.GenericLinks.Extensions;
using Geta.Optimizely.GenericLinks.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using EPiServer.Core;
using SystemTextJsonIgnore = System.Text.Json.Serialization.JsonIgnoreAttribute;
using NewtonsoftJsonIgnore = Newtonsoft.Json.JsonIgnoreAttribute;

namespace Geta.Optimizely.GenericLinks;

public abstract class LinkData : ILinkData
{
    private readonly IDictionary<string, string> _attributes;
    private readonly IDictionary<string, string> _attributeKeys;
    private readonly object _pessimisticLock;

    private bool _isModified;
    private bool _isReadOnly;
    private string? _text;

    protected LinkData()
    {
        _attributes = new Dictionary<string, string>(4);
        _attributeKeys = new Dictionary<string, string>(4);
        _pessimisticLock = new object();
    }

    [Display(Order = 100)]
    public virtual string? Text
    {
        get => _text;
        set
        {
            if (value == _text)
                return;

            SetModified(true);
            _text = value;
        }
    }

    [Display(Order = 200)]
    public virtual string? Title
    {
        get => GetAttribute();
        set => SetAttribute(value);
    }

    [Display(Order = 300)]
    public virtual string? Target
    {
        get => GetAttribute();
        set => SetAttribute(value);
    }

    [Display(Order = 400)]
    public virtual string? Href
    {
        get => GetAttribute();
        set
        {
            UriHelper.CreateUri(value);
            SetAttribute(value);
        }
    }

    [ScaffoldColumn(false)]
    public virtual IDictionary<string, string> Attributes => _attributes;

    [Ignore]
    [SystemTextJsonIgnore]
    [NewtonsoftJsonIgnore]
    public virtual bool IsModified
    {
        get => _isModified;
    }

    [Ignore]
    [ScaffoldColumn(false)]
    [SystemTextJsonIgnore]
    [NewtonsoftJsonIgnore]
    public virtual IList<Guid> ReferencedPermanentLinkIds
    {
        get
        {
            var href = Href;

            if (string.IsNullOrEmpty(href))
                return [];

            var guid = PermanentLinkUtility.GetGuid(href);
            if (guid == Guid.Empty)
                return [];

            return [guid];
        }
    }

    public virtual IDictionary<string, string> GetAttributes()
    {
        return Attributes;
    }

    public virtual void SetAttributes(IDictionary<string, string> attributes)
    {
        foreach (var attribute in attributes)
        {
            Attributes.TryGetValue(attribute.Key, out var existingValue);
            if (existingValue == attribute.Value)
                continue;

            Attributes[attribute.Key] = attribute.Value;
        }
    }

    public virtual void SetModified(bool isModified)
    {
        _isModified = isModified;
    }

    public virtual void RemapPermanentLinkReferences(IDictionary<Guid, Guid> idMap)
    {
        var href = Href;
        if (string.IsNullOrEmpty(href))
            return;

        var guid = PermanentLinkUtility.GetGuid(href);
        if (guid == Guid.Empty)
            return;

        if (!idMap.TryGetValue(guid, out var value))
            return;

        if (value == Guid.Empty)
            value = idMap[guid] = Guid.NewGuid();

        Href = PermanentLinkUtility.ChangeGuid(href, value);
    }

    public virtual object Clone()
    {
        if (Activator.CreateInstance(GetType()) is not LinkData item)
            throw new InvalidOperationException("Cloned instance must inherit from LinkData");

        item.Text = Text;
        item.SetAttributes(Attributes);
        item.SetModified(IsModified);

        return item;
    }

    //
    // Summary:
    //     Gets and sets the read-only property.
    //
    // Value:
    //     true if this property is read only; otherwise, false.
    public bool IsReadOnly
    {
        get
        {
            return _isReadOnly;
        }
        protected set
        {
            _isReadOnly = value;
        }
    }

    //
    // Summary:
    //     Creates a writable clone of the property.
    //
    // Returns:
    //     A writable copy of the property.
    public virtual ILinkData CreateWritableClone()
    {
        return CreateWritableCloneInternal();
    }
    object IReadOnly.CreateWritableClone()
    {
        return CreateWritableCloneInternal();
    }
    private LinkData CreateWritableCloneInternal()
    {
        var obj = (LinkData)Clone();
        obj._isReadOnly = false;
        return obj;
    }

    //
    // Summary:
    //     Convert this property to ReadOnly
    //
    // Remarks:
    //     Implementors should override this method when exposing complex objects that should
    //     be read-only as well.
    public virtual void MakeReadOnly()
    {
        if (!IsReadOnly)
        {
            _isModified = true;
            _isReadOnly = true;
        }
    }

    //
    // Summary:
    //     Convert this property to ReadOnly
    //
    // Remarks:
    //     Implementors should override this method when exposing complex objects that should
    //     be read-only as well.
    public virtual void ResetModified()
    {
        if (!IsModified)
        {
            _isModified = false;
        }
    }

    //
    // Summary:
    //      Used to return references to internal content links

    public virtual IEnumerable<ContentReference> GetReferencedContent()
    {
        return [];
    }

    //
    // Summary:
    //     Checks if the property is read-only
    protected void ThrowIfReadOnly()
    {
        if (IsReadOnly)
        {
            throw new NotSupportedException("LinkData is read-only.");
        }
    }

    protected virtual string? GetAttribute([CallerMemberName] string? key = null)
    {
        if (key is null)
            return null;

        if (Attributes.TryGetValue(GetAttributeKey(key), out var value))
            return value;

        return null;
    }

    protected virtual T? GetAttribute<T>(Func<string, T> conversion, [CallerMemberName] string? key = null)
    {
        if (key is null)
            return default;

        Attributes.TryGetValue(GetAttributeKey(key), out var value);

        if (string.IsNullOrEmpty(value))
            return default;

        return conversion(value);
    }

    protected virtual void SetAttribute(string? value, [CallerMemberName] string? key = null)
    {
        if (key is null)
            return;

        var attributeKey = GetAttributeKey(key);

        if (value is null)
        {
            if (!Attributes.ContainsKey(attributeKey))
                return;

            Attributes.Remove(attributeKey);
            _isModified = true;
        }
        else
        {
            Attributes.TryGetValue(attributeKey, out var existingValue);
            if (existingValue == value)
                return;

            Attributes[attributeKey] = value;
            _isModified = true;
        }
    }

    protected virtual void SetAttribute<T>(T? value, Func<T, string> conversion, [CallerMemberName] string? key = null)
    {
        if (conversion is null)
            return;

        if (key is null)
            return;

        if (value is null)
        {
            SetAttribute(null, key);
        }
        else
        {
            var stringValue = conversion(value);
            SetAttribute(stringValue, key);
        }
    }

    protected virtual string GetAttributeKey(string key)
    {
        if (_attributeKeys.TryGetValue(key, out var attributeKey))
            return attributeKey;

        lock (_pessimisticLock)
        {
            if (_attributeKeys.ContainsKey(key))
                return _attributeKeys[key];

            attributeKey = key.ToCamel();
            _attributeKeys.Add(key, attributeKey);

            return attributeKey;
        }
    }
}
