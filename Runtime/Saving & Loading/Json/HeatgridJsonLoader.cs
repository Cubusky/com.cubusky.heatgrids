using UnityEngine;

namespace Cubusky.Heatgrids
{
    public sealed class HeatgridJsonLoader : HeatgridLoader
    {
        [SerializeField] private JsonDirectoryLoader _loader = new();

        public override IHeatgridLoader loader => _loader;
    }
}
