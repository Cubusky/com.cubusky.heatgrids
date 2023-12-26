using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cubusky.Heatgrids
{
    public sealed class HeatgridDrawer : MonoBehaviour, IHeatgrid, IHeatgridVisualizer
    {
        [SerializeField, OfType(typeof(IHeatgrid))] private Object _grid;
        [SerializeField, OfType(typeof(IHeatgridVisualizer))] private Object _visualizer;

        private IHeatgrid heatgrid
        {
            get => _grid as IHeatgrid;
            set => _grid = value as Object;
        }

        private IHeatgridVisualizer visualizer
        {
            get => _visualizer as IHeatgridVisualizer;
            set => _visualizer = value as Object;
        }

        Dictionary<Vector3Int, int> IHeatgrid.grid => heatgrid.grid;
        float IHeatgrid.cellSize => heatgrid.cellSize;

        StepGradient IHeatgridVisualizer.stepGradient => visualizer.stepGradient;
        void IHeatgridVisualizer.Visualize(IHeatgrid heatgrid) => visualizer.Visualize(heatgrid);

        [ContextMenu(nameof(SetMinMax))]
        private void SetMinMax()
        {
            visualizer.stepGradient.minSteps = heatgrid.grid.Values.Min();
            visualizer.stepGradient.maxSteps = heatgrid.grid.Values.Max();
        }

        [ContextMenu(nameof(SetMinMax80))]
        private void SetMinMax80()
        {
            var min = heatgrid.grid.Values.Min();
            var max = heatgrid.grid.Values.Max();
            var delta = max - min;
            var delta20 = (int)(delta * 0.2f);

            visualizer.stepGradient.minSteps = min + delta20;
            visualizer.stepGradient.maxSteps = max - delta20;
        }

        [ContextMenu(nameof(SetAverage80))]
        private void SetAverage80()
        {
            var average = heatgrid.grid.Values.Average();

            visualizer.stepGradient.minSteps = (int)System.Math.Ceiling(average * 0.2);
            visualizer.stepGradient.maxSteps = (int)System.Math.Ceiling(average * 1.8);
        }

        [ContextMenu(nameof(Draw))]
        private void Draw() => visualizer.Visualize(this);
    }
}
