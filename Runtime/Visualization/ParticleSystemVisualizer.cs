using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Cubusky.Heatgrids
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemVisualizer : MonoBehaviour, ISerializationCallbackReceiver, IVisualizer
    {
        [field: SerializeField, HideInInspector] public new ParticleSystem particleSystem { get; private set; }

        [field: Header("Loading")]
        [field: SerializeReference, ReferenceDropdown] public IEnumerableLoader loader { get; set; }
        [field: SerializeReference, ReferenceDropdown(true)] public IFilter filter { get; set; }
        [field: SerializeField] public int maxParticleGrowth { get; set; } = 100_000;

        [field: Header("Visualization")]
        [field: SerializeField] public StepGradient stepGradient { get; set; }
        [field: SerializeField] public float sizeMultiplier { get; set; } = 2.5f;

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

        private void Reset()
        {
            InitializeComponents();

#if UNITY_EDITOR
            ParticleSystemRenderer renderer = GetComponent<ParticleSystemRenderer>();
            renderer.sortingFudge = -1_000_000f;
            if (!renderer.sharedMaterial)
            {
                const string materialName = "Default-Particle";
                renderer.sharedMaterial = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>($"{materialName}.mat");
            }
#endif
        }

        private void Awake()
        {
            InitializeComponents();
        }

        private void Visualize(ParticleSystem.Particle[] particles)
        {
            particleSystem.Play();
            particleSystem.SetParticles(particles);
            particleSystem.Pause();
        }

        #region Serialization & Initialization
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            try
            {
                // This needs to happen in the serialization step, otherwise the particles will start moving the moment the editor reloads.
                UpdateParticleSystem();
            }
            catch (NullReferenceException) { }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() { }

        private void UpdateParticleSystem()
        {
            var colorBySpeed = particleSystem.colorBySpeed;
            var colorBySpeedColor = colorBySpeed.color;
            colorBySpeedColor.gradient = stepGradient.gradient;
            colorBySpeed.color = colorBySpeedColor;

            var sizeBySpeed = particleSystem.sizeBySpeed;
            colorBySpeed.range = sizeBySpeed.range = new Vector2(stepGradient.minSteps, stepGradient.maxSteps);

            var sizeOverLifetime = particleSystem.sizeOverLifetime;
            sizeOverLifetime.sizeMultiplier = sizeMultiplier;

            particleSystem.Pause();
        }
        #endregion

        #region Particle Population
        private ParticleSystem.Particle[] particles = null;
        private bool isPopulatingParticles;
        private event Action<ParticleSystem.Particle[]> particlesPopulated;

        private static void SetParticle(ref ParticleSystem.Particle particle, Vector3Int coordinates, int steps, float cellSize)
        {
            // Set Starting Values.
            particle.position = IHeatgrid.GridToWorld(coordinates, cellSize);
            particle.startSize = cellSize;
            particle.startColor = Color.white;
            particle.startLifetime = float.PositiveInfinity;

            // Set Speed. We use this to apply the color dynamically.
            particle.velocity = Vector3.forward * steps;
        }

        private async void PopulateParticlesAsync()
        {
            if (isPopulatingParticles)
            {
                return;
            }

            isPopulatingParticles = true;

            try
            {
                var gridsByCellSizes = new SortedList<float, Dictionary<Vector3Int, int>>();

                await foreach (var json in loader.LoadAsyncEnumerable<IEnumerable<string>>(destroyCancellationToken).ConfigureAwait(false))
                {
                    // Update gridsByCellSizes
                    await Task.Run(() =>
                    {
                        var grid = JsonHeatgrid.FromJson(json, out var cellSize);
                        if (filter?.Include(new Heatgrid() { grid = grid, cellSize = cellSize }) == false)
                        {
                            return;
                        }

                        var smallerGrids = gridsByCellSizes.TakeWhile(gridByCellSize => gridByCellSize.Key < cellSize).Select(gridByCellSize => gridByCellSize.Value);
                        void AddCellToSmallerGrids(KeyValuePair<Vector3Int, int> cell)
                        {
                            foreach (var smallerGrid in smallerGrids)
                            {
                                if (smallerGrid.ContainsKey(cell.Key))
                                {
                                    smallerGrid[cell.Key] += cell.Value;
                                }
                            }
                        }

                        if (!gridsByCellSizes.ContainsKey(cellSize))
                        {
                            foreach (var cell in grid)
                            {
                                AddCellToSmallerGrids(cell);
                            }

                            gridsByCellSizes.Add(cellSize, grid);
                        }
                        else
                        {
                            foreach (var cell in grid)
                            {
                                AddCellToSmallerGrids(cell);

                                if (!gridsByCellSizes[cellSize].TryAdd(cell.Key, cell.Value))
                                {
                                    gridsByCellSizes[cellSize][cell.Key] += cell.Value;
                                }
                            }
                        }
                    }, destroyCancellationToken);

                    // Populate particles
                    await Task.Run(() =>
                    {
                        particles = new ParticleSystem.Particle[0];
                        int particleIndex = 0;

                        foreach (var gridByCellsize in gridsByCellSizes)
                        {
                            Array.Resize(ref particles, particles.Length + gridByCellsize.Value.Count);

                            foreach (var cell in gridByCellsize.Value)
                            {
                                SetParticle(ref particles[particleIndex], cell.Key, cell.Value, gridByCellsize.Key);
                                particleIndex++;
                            }
                        }
                    }, destroyCancellationToken);

                    particlesPopulated?.Invoke(particles);
                }
            }
            finally
            {
                isPopulatingParticles = false;
                particlesPopulated = null;
            }
        }
        #endregion

        #region Context Methods
        [ContextMenu(nameof(SetAverageSteps))]
        private void SetAverageSteps()
        {
            //var average = gridsByCellsizes.Values.SelectMany(grid => grid.Values).Average();
            var average = particles.Average(particle => particle.velocity.magnitude);
            stepGradient.minSteps = (int)Math.Ceiling(average * 0.2);
            stepGradient.maxSteps = (int)Math.Ceiling(average * 1.8);
        }

        [ContextMenu(nameof(SetMaxParticles))]
        private void SetMaxParticles()
        {
            var main = particleSystem.main;
            if ((main.maxParticles = particles.Length) > maxParticleGrowth)
            {
                Debug.LogWarning($"Max Particles have been set to {particles.Length}. Note that a large amount of particles may impact editor performance.");
            }
        }

        [ContextMenu(nameof(Visualize))]
        public void Visualize()
        {
            if (particles != null)
            {
                Visualize(particles);
            }
            else
            {
                particlesPopulated += GrowThenVisualize;
                PopulateParticlesAsync();

                void GrowThenVisualize(ParticleSystem.Particle[] particles)
                {
                    var main = particleSystem.main;
                    if (particles.Length >= maxParticleGrowth)
                    {
                        main.maxParticles = maxParticleGrowth;
                        particlesPopulated -= GrowThenVisualize;
                    }
                    else
                    {
                        main.maxParticles = particles.Length;
                    }

                    Visualize(particles);
                }
            }
        }

        [ContextMenu(nameof(Stop))]
        private void Stop()
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particles = null;
        }
        #endregion
    }
}
