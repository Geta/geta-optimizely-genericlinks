# Geta.GenericLinks

## What is does

Provides a new property type `LinkDataCollection` which is an alternative to `LinkItemCollection` which can be extended with additional propertes.

## How to get started?

Requires using .NET 5.0 or higher and Optimizely 12

- `install-package Geta.GenericLinks`

First, implement `LinkData` on a derived type.

```
public class ThumbnailLinkData : LinkData
{
    [Display(Name = "Thumbnail image", Order = 0)]
    [UIHint(UIHint.Image)]
    public virtual ContentReference? Thumbnail
    {
        get
        {
            var attribute = GetAttribute();

            if (ContentReference.TryParse(attribute, out var reference))
                return reference;

            return null;
        }
        set
        {
            SetAttribute(value);
        }
    }

    ...
}
```

Then create a property definition

```
[PropertyDefinitionTypePlugIn(DisplayName = "PropertyThumbnailCollection")]
public class PropertyThumbnailCollection : PropertyLinkDataCollection<ThumbnailLinkData>
{

}
```

Then register the property on a content model

```
[CultureSpecific]
[Display(Name = "Thumbnail links", GroupName = SystemTabNames.Content, Order = 230)]
[BackingType(typeof(PropertyThumbnailCollection))]
public virtual LinkDataCollection<ThumbnailLinkData> Thumbnails { get; set; }
```

## Package maintainer

https://github.com/svenrog

## Changelog

[Changelog](CHANGELOG.md)
