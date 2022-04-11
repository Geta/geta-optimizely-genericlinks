# Implement a link data collection featuring a thumbnail

![Editor result](./images/thumbnail-link-modal.png)

## Inherit LinkData

First, implement a class inheriting from `LinkData`. Lets call it `ThumbnailLinkData`.

Since backing data is stored in a `Dictionary<string, string>` called `Attributes`.
Properties need to read and write from that.
This can be achieved by using the various overloads for the method `GetAttribute` and `SetAttribute` like illustrated below.
Dictionary key resolution and null values are handled behind the scenes.

```
public class ThumbnailLinkData : LinkData
{
    [Display(Name = "Thumbnail image", Order = 0)]
    [UIHint("image")]
    public virtual ContentReference Thumbnail
    {
        get => GetAttribute((v) => ContentReference.Parse(v));
        set => SetAttribute(value, (v) => v.ToString());
    }
}
```

### Supported property types

There are some limitations on which types that have support out of the box.

| Type             | Supported |
| ---------------- | --------- |
| string           | ✅        |
| ContentReference | ✅        |
| int              | ✅        |
| int?             | ✅        |
| double           | ✅        |
| double?          | ✅        |
| decimal          | ✅        |
| decimal?         | ✅        |
| DateTime         | ✅        |
| DateTime?        | ✅        |
| BlockData        | ❌        |

To add support for additional properties please refer to [this guide](./adding-support-for-new-properties.md).

### Sorting of properties

To control sorting of the property, `Order` in the `[Display]` data annotation attribute controls in which order the property is displayed.
`LinkData` already defines some properties and sort orders, the `Order` property can be used to place a property anywhere in relation to these.

| Property | Sort order |
| -------- | ---------- |
| Text     | 100        |
| Title    | 200        |
| Target   | 300        |
| Href     | 400        |

### Validation attributes

To control validation of properties validation attributes like `[Required]` or `[Range]` can be used to decorate property property definitions in `LinkData`.

### Hiding existing properties

Existing properties can be hidden by overriding them and adding `[ScaffoldColumn(false)]`.

```
[ScaffoldColumn(false)]
public override string Target
{
    get => base.Target;
    set => base.Target = value;
}
```

## Create a property definition

Create a property definition that inherits from `PropertyLinkDataCollection` using your `LinkData` implementation as a generic type parameter.

```
[PropertyDefinitionTypePlugIn(DisplayName = "Link collection with thumbnails", GUID = "9f711ce6-ee75-466c-ab9c-67b65a85abc1")]
public class PropertyThumbnailCollection : PropertyLinkDataCollection<ThumbnailLinkData>
{

}
```

## Define a property on another content model

Define a new property with `LinkDataCollection<ThumbnailLinkData>` on content.
The backing type will resolve automatically.

```
[CultureSpecific]
[Display(Name = "Thumbnail links", Order = 230)]
public virtual LinkDataCollection<ThumbnailLinkData> Thumbnails { get; set; }
```

![Property looks like this](./images/thumbnal-links.png)
