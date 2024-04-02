using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cubusky.Heatgrids
{
    public class HeatgridRecorder : MonoBehaviour, IHeatgrid, ISerializationCallbackReceiver
    {
        [SerializeField, TimeSpan] private long _minimumRecordingTime = TimeSpan.TicksPerSecond * 10;
        [field: SerializeReference, ReferenceDropdown] public IHeatgrid heatgrid { get; set; }
        [field: SerializeField] public bool saveInEditor { get; set; }
        [field: SerializeReference, ReferenceDropdown(nullable = true)] public ICompressor compressor { get; set; }
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

        private async void OnDestroy()
        {
#if UNITY_EDITOR
            if (saveInEditor)
#endif
            {
                if (recordingTime > minimumRecordingTime.TotalSeconds)
                {
                    var json = JsonHeatgrid.ToJson(heatgrid);
                    var bytes = saver.encoding.GetBytes(json);
                    bytes = compressor != null
                        ? await compressor.CompressAsync(bytes)
                        : bytes;
                    await saver.SaveAsync(bytes);
                }
            }
        }
    }
}
