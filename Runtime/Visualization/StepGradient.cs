using System;
using UnityEngine;

namespace Cubusky.Heatgrids
{
    [Serializable]
    public class StepGradient : ISerializationCallbackReceiver
    {
        [SerializeField, Min(0)] private int _minSteps = 1;
        [SerializeField, Min(1)] private int _maxSteps = 8;
        private float _deltaSteps = 1f;

        public int minSteps
        {
            get => _minSteps;
            set => _deltaSteps = Mathf.Max(maxSteps - (_minSteps = Mathf.Max(value, 0)), 1);
        }

        public int maxSteps
        {
            get => _maxSteps;
            set => _deltaSteps = Mathf.Max((_maxSteps = Mathf.Max(value, 1)) - minSteps, 1);
        }

        [field: SerializeField]
        public Gradient gradient { get; set; } = new Gradient()
        {
            colorKeys = new GradientColorKey[]
            {
                new(Color.blue, 0f),
                new(Color.green, 0.5f),
                new(Color.red, 1f),
            },
            colorSpace = ColorSpace.Linear,
            mode = GradientMode.PerceptualBlend
        };

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _deltaSteps = Mathf.Max(maxSteps - minSteps, 1);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() { }

        public Color Evaluate(int steps) => gradient.Evaluate((steps - minSteps) / _deltaSteps);
    }
}
