using System;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Cubusky.Heatgrids
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemVisualizer : MonoBehaviour, IHeatgridVisualizer, ISerializationCallbackReceiver
    {
        [field: SerializeField, HideInInspector] public new ParticleSystem particleSystem { get; private set; }
        [field: SerializeField] public StepGradient stepGradient { get; set; }
        [field: SerializeField] public float sizeMultiplier { get; set; } = 2.5f;

        private void Reset()
        {
            InitializeComponents();

#if UNITY_EDITOR
            ParticleSystemRenderer renderer = GetComponent<ParticleSystemRenderer>();
            if (!renderer.sharedMaterial)
            {
                const string materialName = "Default-Particle";
                renderer.sharedMaterial = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>($"{materialName}.mat");
                Debug.Log($"Assigned \"{materialName}\" to the {nameof(ParticleSystem)}'s {nameof(Material)}. Note that instantiating {nameof(ParticleSystemVisualizer)}'s from script does not automatically assign a {nameof(Material)} to the {nameof(ParticleSystem)}.", this);
            }
#endif
        }

        private void Awake()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            particleSystem = GetComponent<ParticleSystem>();

            var emission = particleSystem.emission;
            emission.enabled = false;

            var shape = particleSystem.shape;
            shape.enabled = false;

            var colorBySpeed = particleSystem.colorBySpeed;
            colorBySpeed.enabled = true;

            var sizeOverLifetime = particleSystem.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            var sizeOverLifetimeSize = sizeOverLifetime.size;
            sizeOverLifetimeSize.curve = AnimationCurve.Constant(0f, 1f, 1f);
            sizeOverLifetime.size = sizeOverLifetimeSize;

            var main = particleSystem.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.playOnAwake = main.loop = false;

            var particleSystemRenderer = GetComponent<ParticleSystemRenderer>();
            particleSystemRenderer.sortMode = ParticleSystemSortMode.Distance;
            particleSystemRenderer.allowRoll = false;
            particleSystemRenderer.alignment = ParticleSystemRenderSpace.Facing;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            try
            {
                var colorBySpeed = particleSystem.colorBySpeed;
                var colorBySpeedColor = colorBySpeed.color;
                colorBySpeedColor.gradient = stepGradient.gradient;
                colorBySpeed.color = colorBySpeedColor;

                var sizeBySpeed = particleSystem.sizeBySpeed;
                colorBySpeed.range = sizeBySpeed.range = new Vector2(stepGradient.minSteps, stepGradient.maxSteps);

                var sizeOverLifetime = particleSystem.sizeOverLifetime;
                sizeOverLifetime.sizeMultiplier = sizeMultiplier;
            }
            catch (NullReferenceException) { }

            particleSystem.Pause();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() { }

        private Particle[] particles;

        void IHeatgridVisualizer.Visualize(IHeatgrid heatgrid)
        {
            if (heatgrid.grid.Count == 0)
            {
                return;
            }

            particles = new Particle[heatgrid.grid.Count];
            int particleIndex = 0;

            foreach (var cell in heatgrid.grid)
            {
                if (particleIndex == particleSystem.main.maxParticles)
                {
                    Debug.LogWarning($"The {nameof(ParticleSystemVisualizer)} won't draw any further because the {nameof(particleSystem.main.maxParticles)} has been reached. Consider increasing the {nameof(particleSystem.main.maxParticles)} inside the {nameof(ParticleSystem)}.", this);
                    break;
                }

                // Set Starting Values.
                particles[particleIndex].position = heatgrid.GridToWorld(cell.Key);
                particles[particleIndex].startSize = heatgrid.cellSize;
                particles[particleIndex].startColor = Color.white;
                particles[particleIndex].startLifetime = float.PositiveInfinity;

                // Set Speed. We use this to apply the color dynamically.
                particles[particleIndex].velocity = Vector3.forward * cell.Value;

                particleIndex++;
            }

            particleSystem.Play();
            particleSystem.SetParticles(particles);
            particleSystem.Pause();
        }
    }
}
