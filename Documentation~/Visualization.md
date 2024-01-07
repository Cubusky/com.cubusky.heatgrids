# Visualization

Visualizers can be custom implemented through the `IHeatgridVisualizer` interface.

## Particle Solution

Heatgrids comes builtin with a particle solution for visualizing heatgrids. Simply add a `ParticleSystemVisualizer` to your scene and select an `IHeatgrid` to visualize. You can play with the `ParticleSystemVisualizer` settings at runtime to hone in on the data you are particularly interested in.

The [context menu](https://docs.unity3d.com/Manual/UsingComponents.html) (see "Component context menu commands") provides very simple methods to setup your `ParticleSystem`.
- `SetAverageSteps` will set the min- and max steps to a reasonable range.
- `SetMaxParticles` will set the maximum particles needed to visualize the heatgrid.
- `Visualize` will visualize the heatgrid.