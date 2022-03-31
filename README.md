# Geta.GenericLinks

## What is does

Provides a new property type `LinkDataCollection` which is an alternative to `LinkItemCollection`. It can be extended with additional propertes.

## How to get started?

Requires using .NET 5.0 or higher and Optimizely 12

- `install-package Geta.GenericLinks`

First, implement `LinkData` on a derived type.

Note that you can use `DataAnnotation` attributes like `Display` and `UIHint`, data validation attributes also work.

```
public class ThumbnailLinkData : LinkData
{
    [Display(Name = "Thumbnail image", Order = 0)]
    [UIHint(UIHint.Image)]
    [Required]
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

Then create a property definition that inherits from `PropertyLinkDataCollection` with your `LinkData` implementation.

```
[PropertyDefinitionTypePlugIn(DisplayName = "PropertyThumbnailCollection")]
public class PropertyThumbnailCollection : PropertyLinkDataCollection<ThumbnailLinkData>
{

}
```

Then define a new property with `LinkDataCollection<ThumbnailLinkData>` on content. Backing types will resolve automatically by default via `LinkDataBackingTypeResolverInterceptor`.

```
[CultureSpecific]
[Display(Name = "Thumbnail links", GroupName = SystemTabNames.Content, Order = 230)]
public virtual LinkDataCollection<ThumbnailLinkData> Thumbnails { get; set; }
```

## Package maintainer

https://github.com/svenrog

## Changelog

[Changelog](CHANGELOG.md)
