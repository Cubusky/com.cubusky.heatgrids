using System.Collections.Generic;
using UnityEngine;

namespace Cubusky.Heatgrids
{
    public static class HeatgridUtility
    {
        public static Dictionary<Vector3Int, int> MergeGrids(params Dictionary<Vector3Int, int>[] grids)
        {
            if (grids.Length == 0)
            {
                return default;
            }

            var mergedGrid = grids[0];
            for (int i = 1;  i < grids.Length; i++)
            {
                foreach (var cell in grids[i])
                {
                    if (!mergedGrid.TryAdd(cell.Key, cell.Value))
                    {
                        mergedGrid[cell.Key] += cell.Value;
                    }
                }
            }
            return mergedGrid;
        }
    }
}
