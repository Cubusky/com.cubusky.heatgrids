using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cubusky.Heatgrids
{
    public class HeatgridRecorder : MonoBehaviour, IHeatgrid, ISerializationCallbackReceiver
    {
        [SerializeField, TimeSpan] private long _minimumRecordingTime = TimeSpan.TicksPerSecond * 10;
        [field: SerializeReference, ReferenceDropdown] public IHeatgrid heatgrid { get; set; }
        [field: SerializeReference, ReferenceDropdown] public ISaver saver { get; set; }

        Dictionary<Vector3Int, int> IHeatgrid.grid => heatgrid.grid;
        float IHeatgrid.cellSize => heatgrid.cellSize;

        public TimeSpan minimumRecordingTime { get; set; }
        public float recordingTime { get; private set; }

        void ISerializationCallbackReceiver.OnBeforeSerialize() => _minimumRecordingTime = minimumRecordingTime.Ticks;
        void ISerializationCallbackReceiver.OnAfterDeserialize() => minimumRecordingTime = new(_minimumRecordingTime);

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
            if (recordingTime > minimumRecordingTime.TotalSeconds)
            {
                var json = JsonHeatgrid.ToJson(heatgrid);
                saver.SaveAsync(json);
            }
        }
    }
}
