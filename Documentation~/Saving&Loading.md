# Saving & Loading

Saving & Loading is one of the most important aspects of Heatgrids, as it makes decisions about what data is important enough to store, and what data is important enough of collect. Because of this, Heatgrid exposes its functionality through interfaces. Implementations of these interfaces can be selected through the `HeatgridRecorder` and `HeatgridLoader`, according to the requirements of the [ReferenceDropdownAttribute](https://cubusky.github.io/com.cubusky.core/manual/ReferenceDropdownAttribute.html#restrictions).

## Json Solution

With the `JsonFileSaver` inside your `HeatgridRecorder` selected, heatgrids will be saved to Unity's [persistent data path](https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html) in json format.

With the `JsonDirectoryLoader` inside your `HeatgridLoader` selected, heatgrids in json format will be collected from the [persistent data path](https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html) and combined into 1 large heatgrid.

## Custom Solutions

Derive from `IHeatgridSaver` to implement a custom heatgrid saving solution, and derive from `IHeatgridLoader` to implement a custom heatgrid loading solution.

```csharp
public class XmlFileSaver : IHeatgridSaver { ... }
public class XmlDirectoryLoader : IHeatgridLoader { ... }
```

These will be selectable inside the `HeatgridRecorder` and `HeatgridLoader` respectively.