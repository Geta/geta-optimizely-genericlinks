// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.Core;
using EPiServer.Core.Internal;
using EPiServer.Core.Transfer;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Geta.Optimizely.GenericLinks.Html;
using Geta.Optimizely.GenericLinks.Extensions;

namespace Geta.Optimizely.GenericLinks;

public abstract class PropertyLinkDataCollection : PropertyLinkBase
{
    protected PropertyLinkDataCollection()
    {
    }

    protected PropertyLinkDataCollection(string value) : base(value)
    {
    }
}

public abstract class PropertyLinkDataCollection<TLinkData> : PropertyLinkDataCollection, IReferenceMap, IEnumerable<TLinkData>
    where TLinkData : ILinkData, new()
{
    private readonly IUrlResolver _urlResolver;
    private readonly IAttributeSanitizer _attributeSanitizer;
    private readonly ILinkHtmlSerializer _htmlSerializer;

    private LinkDataCollection<TLinkData>? _linkItemCollection;

    protected PropertyLinkDataCollection()
        : this(
               ServiceLocator.Current.GetInstance<IUrlResolver>(),
               ServiceLocator.Current.GetInstance<IAttributeSanitizer>(),
               ServiceLocator.Current.GetInstance<ILinkHtmlSerializer>())
    {
    }

    protected PropertyLinkDataCollection(LinkDataCollection<TLinkData> linkItemCollection)
        : this(
              linkItemCollection,
               ServiceLocator.Current.GetInstance<IUrlResolver>(),
               ServiceLocator.Current.GetInstance<IAttributeSanitizer>(),
               ServiceLocator.Current.GetInstance<ILinkHtmlSerializer>())
    {
    }

    protected PropertyLinkDataCollection(
        IUrlResolver urlResolver,
        IAttributeSanitizer attributeSanitizer,
        ILinkHtmlSerializer htmlSerializer)
        : base(string.Empty)
    {
        _urlResolver = urlResolver;
        _attributeSanitizer = attributeSanitizer;
        _htmlSerializer = htmlSerializer;
    }

    protected PropertyLinkDataCollection(
        LinkDataCollection<TLinkData> linkItemCollection,
        IUrlResolver urlResolver,
        IAttributeSanitizer attributeSanitizer,
        ILinkHtmlSerializer htmlSerializer)
    {
        _linkItemCollection = linkItemCollection;
        _urlResolver = urlResolver;
        _attributeSanitizer = attributeSanitizer;
        _htmlSerializer = htmlSerializer;
    }

    [XmlIgnore]
    public virtual LinkDataCollection<TLinkData>? Links
    {
        get => _linkItemCollection;
        set
        {
            ThrowIfReadOnly();

            if (value is null)
            {
                Clear();
            }
            else if (_linkItemCollection != value)
            {
                _linkItemCollection = value;
                ModifiedNoCheck();
            }
        }
    }

    public override bool IsModified
    {
        get
        {
            if (base.IsModified)
                return true;

            if (_linkItemCollection is not null)
                return _linkItemCollection.IsModified;

            return false;
        }
        set => base.IsModified = value;
    }

    public override bool IsNull
    {
        get
        {
            if (_linkItemCollection is not null && _linkItemCollection.Count > 0)
                return false;

            return !((ILazyProperty)this).HasLazyValue;
        }
    }

    public override PropertyDataType Type => PropertyDataType.LinkCollection;
    public override Type PropertyValueType => typeof(LinkDataCollection<TLinkData>);

    public override object? Value
    {
        get
        {
            if (_linkItemCollection is null)
            {
                LoadData(base.LongString);
            }

            return _linkItemCollection;
        }
        set
        {
            SetPropertyValue(value, delegate
            {
                var collection = value as LinkDataCollection<TLinkData>;
                if (collection is not null || value is null)
                {
                    Links = collection;
                }
                else
                {
                    if (value is not string text)
                        throw new ArgumentException("Passed object must be of type LinkItemCollection<TLinkData> or string.", nameof(value));

                    ParseToSelf(text);
                }
            });
        }
    }
    protected override string LongString
    {
        get => _htmlSerializer.Serialize(_linkItemCollection, StringMode.InternalMode);
        set => base.LongString = value;
    }

    public override void ParseToSelf(string? value)
    {
        ThrowIfReadOnly();

        if (QualifyAsNullString(value))
        {
            _linkItemCollection = null;
            return;
        }

        if (value is null)
            throw new InvalidOperationException("value cannot be null");

        _linkItemCollection = ParseToLinkCollection(value);
        ModifiedNoCheck();
    }

    public override void MakeReadOnly()
    {
        base.MakeReadOnly();

        if (_linkItemCollection is null)
            return;

        if (_linkItemCollection.IsReadOnly)
            return;

        _linkItemCollection.MakeReadOnly();
    }

    public override string ToWebString()
    {
        return _htmlSerializer.Serialize(_linkItemCollection, StringMode.ViewMode);
    }

    public virtual IList<Guid>? ReferencedPermanentLinkIds
    {
        get
        {
            if (IsNull)
                return Array.Empty<Guid>();

            return _linkItemCollection?.ReferencedPermanentLinkIds;
        }
    }

    public override PropertyData CreateWritableClone()
    {
        var clone = (PropertyLinkDataCollection<TLinkData>)base.CreateWritableClone();

        if (_linkItemCollection is not null)
            clone.SetLinkItems(_linkItemCollection.CreateWritableClone());

        return clone;
    }

    public override PropertyData Copy()
    {
        var collection = (PropertyLinkDataCollection<TLinkData>)base.Copy();

        LinkDataCollection<TLinkData> links;

        if (_linkItemCollection is null)
        {
            links = [];
        }
        else
        {
            links = [.. _linkItemCollection];
        }

        collection.SetLinkItems(links);

        if (IsReadOnly)
        {
            links.MakeReadOnly();
        }

        return collection;
    }

    public override void LoadData(object? value)
    {
        if (value is null)
        {
            _linkItemCollection = [];
        }
        else if (value is LinkDataCollection<TLinkData>)
        {
            Value = value;
        }
        else
        {
            _linkItemCollection = ParseToLinkCollection((string)value);
        }
    }

    public virtual void RemapPermanentLinkReferences(IDictionary<Guid, Guid> idMap)
    {
        if (_linkItemCollection is null)
            return;

        _linkItemCollection.RemapPermanentLinkReferences(idMap);
    }

    public virtual IEnumerator<TLinkData> GetEnumerator()
    {
        if (_linkItemCollection is null)
            return Enumerator.Empty<TLinkData>();

        return _linkItemCollection.GetEnumerator();
    }

    protected override void SetDefaultValue()
    {
        base.SetDefaultValue();
        _linkItemCollection = [];
    }

    protected virtual LinkDataCollection<TLinkData>? ParseToLinkCollection(string? value)
    {
        var linkItemCollection = new LinkDataCollection<TLinkData>();

        if (string.IsNullOrEmpty(value))
            return linkItemCollection;

        try
        {
            var linkElements = GetLinkElements(value);

            foreach (var element in linkElements)
            {
                var linkItem = new TLinkData
                {
                    Text = element.Value
                };

                ParseAttributes(element, linkItem);
                linkItemCollection.Add(linkItem);
            }

            return linkItemCollection;
        }
        catch (XmlException xmlException)
        {
            throw new InvalidPropertyValueException(nameof(PropertyLinkDataCollection<TLinkData>), value, xmlException);
        }
        catch (ArgumentException argumentException)
        {
            throw new InvalidPropertyValueException(nameof(PropertyLinkDataCollection<TLinkData>), value, argumentException);
        }
    }

    protected override string? ToLongString()
    {
        if (_linkItemCollection is null)
            return null;

        if (_linkItemCollection.Count == 0)
            return null;

        var elements = new List<XElement>(_linkItemCollection.Count);

        foreach (var linkItem in _linkItemCollection)
        {
            elements.Add(CreateLinkElement(linkItem));
        }

        return new XElement("links", elements).ToString(SaveOptions.DisableFormatting);
    }

    protected override string GetPermanentUrl(string href)
    {
        return _urlResolver.GetPermanent(href, enableFallback: true);
    }

    protected override string SanitizeValue(string value)
    {
        return _attributeSanitizer.Sanitize(value);
    }
    internal void SetLinkItems(LinkDataCollection<TLinkData> linkItems)
    {
        _linkItemCollection = linkItems;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        if (_linkItemCollection is null)
            return Enumerator.Empty<TLinkData>();

        return _linkItemCollection.GetEnumerator();
    }
}
