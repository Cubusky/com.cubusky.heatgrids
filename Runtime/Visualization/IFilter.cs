using System;
using UnityEngine;

namespace Cubusky.Heatgrids
{
    public interface IFilter
    {
        bool Include(IHeatgrid heatgrid);
    }

    [Serializable]
    public class SizeFilter : IFilter
    {
        [field: SerializeField] public float cellSize { get; set; }

        public bool Include(IHeatgrid heatgrid) => heatgrid.cellSize == cellSize;
    }

    [Serializable]
    public class RangeFilter : IFilter
    {
        [field: SerializeField] public Vector2 cellSizeRange { get; set; }

        public bool Include(IHeatgrid heatgrid) => heatgrid.cellSize >= cellSizeRange.x && heatgrid.cellSize <= cellSizeRange.y;
    }
}
