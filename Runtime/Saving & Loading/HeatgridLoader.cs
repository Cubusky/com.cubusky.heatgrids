using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cubusky.Heatgrids
{
    [Obsolete("HeatgridLoader merges grids into one single grid, which is a fundamentally incorrect implementation. This MonoBehaviour will therefor be removed in a later version in favor of letting Visualizers load heatgrids for themselves.", false)]
    public class HeatgridLoader : MonoBehaviour, IHeatgrid
    {
        [field: SerializeReference, ReferenceDropdown] public IHeatgridLoader loader { get; set; }

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
