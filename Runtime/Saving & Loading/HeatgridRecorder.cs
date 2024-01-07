using System.Collections.Generic;
using UnityEngine;

namespace Cubusky.Heatgrids
{
    public interface IHeatgridSaver
    {
        void Save(IHeatgrid heatgrid);
    }

    public class HeatgridRecorder : MonoBehaviour, IHeatgrid, IHeatgridSaver
    {
        [field: SerializeField, Min(0f)] public float minimumRecordingTime { get; set; } = 10f;

        private float _recordingTime;
        public float recordingTime
        {
            get => _recordingTime;
            private set => _recordingTime = value;
        }

        [field: SerializeReference, ReferenceDropdown] public IHeatgrid heatgrid { get; set; }
        [field: SerializeReference, ReferenceDropdown] public IHeatgridSaver saver { get; set; }

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
