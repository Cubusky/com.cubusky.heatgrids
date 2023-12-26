namespace Cubusky.Heatgrids
{
    public interface IHeatgridVisualizer
    {
        StepGradient stepGradient { get; }
        public void Visualize(IHeatgrid heatgrid);
    }
}
