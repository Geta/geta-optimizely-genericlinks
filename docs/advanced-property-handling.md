# Advanced property handling

## Supported property types

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

## Sorting of properties

To control sorting of the property, `Order` in the `[Display]` data annotation attribute controls in which order the property is displayed.
`LinkData` already defines some properties and sort orders, the `Order` property can be used to place a property anywhere in relation to these.

| Property | Sort order |
| -------- | ---------- |
| Text     | 100        |
| Title    | 200        |
| Target   | 300        |
| Href     | 400        |

## Validation attributes

To control validation of properties validation attributes like `[Required]` or `[Range]` can be used to decorate property property definitions in `LinkData`.

## Hiding existing properties

Existing properties can be hidden by overriding them and adding `[ScaffoldColumn(false)]`.

```
[ScaffoldColumn(false)]
public override string Target
{
    get => base.Target;
    set => base.Target = value;
}
```
