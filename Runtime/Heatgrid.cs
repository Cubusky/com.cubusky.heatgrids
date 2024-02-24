using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cubusky.Heatgrids
{
    public interface IHeatgrid
    {
        public Dictionary<Vector3Int, int> grid { get; }
        public float cellSize { get; }

        public Vector3Int WorldToGrid(Vector3 position)
        {
            position /= cellSize;
            return new Vector3Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));
        }

        public Vector3 GridToWorld(Vector3Int coordinates) => cellSize * (Vector3)coordinates;

        [Obsolete("You should not be able to merge grids like this. Instead, you should loop through all grids individually.", false)]
        public static Dictionary<Vector3Int, int> MergeGrids(params Dictionary<Vector3Int, int>[] grids)
        {
            if (grids.Length == 0)
            {
                return default;
            }

            var mergedGrid = grids[0];
            for (int i = 1; i < grids.Length; i++)
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

    [Serializable]
    public class Heatgrid : IHeatgrid
    {
        public Dictionary<Vector3Int, int> grid { get; set; } = new();
        [field: SerializeField, Min(0f)] public float cellSize { get; set; } = 0.5f;
    }
}
