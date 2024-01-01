# Saving & Loading

Saving & Loading is one of the most important aspects of Heatgrids, as it makes decisions about what data is important enough to store, and what data is important enough of collect. Because of this, Heatgrid exposes its functionality through interfaces.

Derive from `IHeatgridSaver` to implement a custom heatgrid saving solution, and derive from `IHeatgridLoader` to implement a custom heatgrid loading solution. Derive from `HeatgridRecorder` and `HeatgridLoader` respectively to expose these through components.

```csharp
[Serializable]
public class XmlFileSaver : IHeatgridSaver { ... }

[Serializable]
public class XmlDirectoryLoader : IHeatgridLoader { ... }

public sealed class HeatgridXmlRecorder : HeatgridRecorder
{
    [SerializeField] private Heatgrid _heatgrid = new();
    [SerializeField] private XmlFileSaver _saver = new();

    public override IHeatgrid heatgrid => _heatgrid;
    public override IHeatgridSaver saver => _saver;
}

public sealed class HeatgridXmlLoader : HeatgridLoader
{
    [SerializeField] private XmlDirectoryLoader _loader = new();

    public override IHeatgridLoader loader => _loader;
}
```

## Json Solution

Heatgrid comes builtin with a Json solution for saving & loading. Simply add a `HeatgridJsonRecorder` to the player character to start recording data about the player. This will save heatgrids to Unity's [persistent data path](https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html).

> [!INFO]
> Be aware that Heatgrids provides no out of the box solution for sending data to a server. It is recommended to implement a solution that sends (new) user data to a server at the end of their playing session.

To load heatgrids from the [persistent data path](https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html), use a `HeatgridJsonLoader` and it will collect all the heatgrids that have been saved and combine them into 1 large heatgrid.