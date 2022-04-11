# Adding support for additional property types

When the client editor model and the backing datatypes don't match up, this library tries to resolve an `ILinkDataValueWriter` for backing -> editor data conversion and `ILinkDataAttributeConverter` for editor -> backing data conversion. The first step is to register a value writer to get the property to show up in edit mode. Secondly if the property doesn't persist between page loads an attribute converter is probably needed too.

## Implement and register an ILinkDataValueWriter

For reference this is the code that makes date pickers display correctly as `System.Text.Json` is used for from Optimizely to Dojo widget data transfer.

```
public class DateTimeValueWriter : ILinkDataValueWriter
{
    public bool CanWrite(Type type)
    {
        if (typeof(DateTime).IsAssignableFrom(type))
            return true;

        if (typeof(DateTime?).IsAssignableFrom(type))
            return true;

        return false;
    }

    public void Write(Utf8JsonWriter writer, object value)
    {
        var dateValue = (DateTime)value;
        writer.WriteStringValue(dateValue);
    }
}
```

Also register it for dependency injection in Startup.

```
services.TryAddEnumerable(ServiceDescriptor.Singleton<ILinkDataValueWriter, DateTimeValueWriter>());
```

## Implement and register an ILinkDataAttributeConverter

This is the code that handles conversions of `IConvertible` datatypes from Dojo Widget to Optimizely.

```
public class ConvertibleAttributeConverter : ILinkDataAttibuteConverter
{
    public bool CanConvert(Type type)
    {
        return typeof(IConvertible).IsAssignableFrom(type);
    }

    public string? Convert(object value)
    {
        if (value is null)
            return null;

        return System.Convert.ToString((IConvertible)value, CultureInfo.InvariantCulture);
    }
}
```

## Going beyond

More information about handling properties by overriding built in services to come.
