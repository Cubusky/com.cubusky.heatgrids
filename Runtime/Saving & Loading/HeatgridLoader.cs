using System.Collections.Generic;
using UnityEngine;

namespace Cubusky.Heatgrids
{
    public interface IHeatgridLoader
    {
        Dictionary<Vector3Int, int> Load(out float cellSize);
    }

    public abstract class HeatgridLoader : MonoBehaviour, IHeatgrid, IHeatgridLoader
    {
        public abstract IHeatgridLoader loader { get; }

        Dictionary<Vector3Int, int> IHeatgridLoader.Load(out float cellSize) => loader.Load(out cellSize);

        private Dictionary<Vector3Int, int> cachedGrid;
        private float cachedCellSize;

        Dictionary<Vector3Int, int> IHeatgrid.grid => cachedGrid ??= loader.Load(out cachedCellSize);
        float IHeatgrid.cellSize
        {
            get
            {
                if (cachedGrid == null)
                {
                    loader.Load(out cachedCellSize);
                }
                return cachedCellSize;
            }
        }

        [ContextMenu(nameof(EmptyCache))]
        public void EmptyCache() => cachedGrid = null;
    }
}
