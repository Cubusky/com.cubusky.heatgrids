using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cubusky.Heatgrids
{
    [Serializable]
    public class JsonHeatgrid
    {
        // WARNING: Never rename this variable or old serialization will break!!!
        public int[] grid;

        public static string ToJson(IHeatgrid heatgrid) => JsonUtility.ToJson(new JsonHeatgrid(heatgrid));
        public static Dictionary<Vector3Int, int> FromJson(string json, out float cellSize) => JsonUtility.FromJson<JsonHeatgrid>(json).ToHeatgrid(out cellSize);

        public JsonHeatgrid(IHeatgrid heatgrid)
        {
            grid = new int[heatgrid.grid.Count * 4 + 2];

            // Simplify grid.
            int i = 0;
            foreach (var cell in heatgrid.grid)
            {
                grid[i++] = cell.Key.x;
                grid[i++] = cell.Key.y;
                grid[i++] = cell.Key.z;
                grid[i++] = cell.Value;
            }

            // Simplify cellSize.
            var scale = heatgrid.cellSize.ToString().Remove(0, heatgrid.cellSize.ToString().IndexOf('.') + 1).Length;
            var upsizedCellSize = (int)(heatgrid.cellSize * Math.Pow(10, scale));
            grid[^2] = upsizedCellSize;
            grid[^1] = scale;
        }

        public Dictionary<Vector3Int, int> ToHeatgrid(out float cellSize)
        {
            if (grid.Length < 2)
            {
                cellSize = default;
                return default;
            }

            // Set cellSize.
            var scale = grid[^1];
            var upsizedCellSize = grid[^2];
            cellSize = (float)(upsizedCellSize / Math.Pow(10, scale));

            // Populate grid.
            var heatgrid = new Dictionary<Vector3Int, int>(grid.Length / 4);
            for (int i = 0; i < grid.Length - 2; i += 4)
            {
                var x = grid[i];
                var y = grid[i + 1];
                var z = grid[i + 2];
                var value = grid[i + 3];

                var key = new Vector3Int(x, y, z);
                heatgrid.Add(key, value);
            }

            return heatgrid;
        }
    }
}
