using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cubusky.Heatgrids
{
    public interface IHeatgrid
    {
        public Dictionary<Vector3Int, int> grid { get; }
        public float cellSize { get; }

        public Vector3Int WorldToGrid(Vector3 position) => WorldToGrid(position, cellSize);
        public static Vector3Int WorldToGrid(Vector3 position, float cellSize)
        {
            position /= cellSize;
            return new Vector3Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));
        }

        public Vector3 GridToWorld(Vector3Int coordinates) => GridToWorld(coordinates, cellSize);
        public static Vector3 GridToWorld(Vector3Int coordinates, float cellSize) => cellSize * (Vector3)coordinates;
    }

    [Serializable]
    public class Heatgrid : IHeatgrid
    {
        public Dictionary<Vector3Int, int> grid { get; set; } = new();
        [field: SerializeField, Min(0f)] public float cellSize { get; set; } = 0.5f;
    }
}
