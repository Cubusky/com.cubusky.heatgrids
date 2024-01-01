# Visualization

Heatgrids can be visualized through the `HeatgridDrawer` component, which takes visualizer and a heatgrid. Simply add a `HeatgridDrawer` to your scene and connect a `IHeatgrid` and a `IHeatgridVisualizer`. Then, right click the `HeatgridDrawer` header to access its context menu and select `SetAverage80`, then `Draw` to draw your heatmap.

![](images/Heatgrid%20Drawer%20Draw.png)

Visualizers can be custom implemented through the `IHeatgridVisualizer` interface.

## Particle Solution

Heatgrids comes builtin with a particle solution for visualizing heatgrids. Simply add a `ParticleSystemVisualizer` to your scene and connect it to a `HeatgridDrawer`. You can play with the `ParticleSystemVisualizer` settings at runtime to hone in on the data you are particularly interested in.