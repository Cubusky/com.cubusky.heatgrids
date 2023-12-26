using System.Collections.Generic;
using UnityEngine;

namespace Cubusky.Heatgrids
{
    public interface IHeatgridSaver
    {
        void Save(IHeatgrid heatgrid);
    }

    public abstract class HeatgridRecorder : MonoBehaviour, IHeatgrid, IHeatgridSaver
    {
        [field: SerializeField, Min(0f)] public float minimumRecordingTime { get; set; } = 10f;

        private float _recordingTime;
        public float recordingTime
        {
            get => _recordingTime;
            private set => _recordingTime = value;
        }

        public abstract IHeatgrid heatgrid { get; }
        public abstract IHeatgridSaver saver { get; }

        Dictionary<Vector3Int, int> IHeatgrid.grid => heatgrid.grid;
        float IHeatgrid.cellSize => heatgrid.cellSize;

        void IHeatgridSaver.Save(IHeatgrid heatgrid) => saver.Save(heatgrid);

        private void Start()
        {
            recordingTime = 0f;
        }

        private void FixedUpdate()
        {
            var coordinates = heatgrid.WorldToGrid(transform.position);
            if (!heatgrid.grid.TryAdd(coordinates, 1))
            {
                heatgrid.grid[coordinates]++;
            }
            recordingTime += Time.fixedDeltaTime;
        }

        private void OnDestroy()
        {
            if (recordingTime >= minimumRecordingTime + float.Epsilon)
            {
                saver.Save(heatgrid);
            }
        }
    }
}
