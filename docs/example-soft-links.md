# Implement soft link references for content reference properties

If you haven't already, please read the guide on how to create a [collection property](example-link-data-collection.md) before continuing.

## Inherit LinkData

Repeat or reuse the steps on inheriting from `LinkData` from the previous guide.

By overriding the method `GetReferencedContent` on `LinkData` a list of content references can be provided to the soft link indexer. This will ensure that appropriate warnings are shown when content is about to be deleted.

```
public class ThumbnailLinkData : LinkData
{
    [Display(Name = "Thumbnail image", Order = 0)]
    [UIHint("image")]
    public virtual ContentReference Thumbnail
    {
        get => GetAttribute(ContentReference.Parse);
        set => SetAttribute(value, v => v.ToString());
    }

    ...

    public override IEnumerable<ContentReference> GetReferencedContent()
    {
        yield return Thumbnail;
    }
}
```
