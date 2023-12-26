using UnityEngine;

namespace Cubusky.Heatgrids
{
    public sealed class HeatgridJsonRecorder : HeatgridRecorder
    {
        [SerializeField] private Heatgrid _heatgrid = new();
        [SerializeField] private JsonFileSaver _saver = new();

        public override IHeatgrid heatgrid => _heatgrid;
        public override IHeatgridSaver saver => _saver;
    }
}
