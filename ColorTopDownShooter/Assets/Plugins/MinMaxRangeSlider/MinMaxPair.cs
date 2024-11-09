using UnityEngine;
using System;

namespace MinMaxRangeSlider
{
    public class MinMaxAttribute : PropertyAttribute
    {
        public float Min, Max;

        public MinMaxAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }

    [Serializable]
    public struct MinMaxPair
    {
        public float Min, Max;

        public MinMaxPair(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public float Clamp(float value)
        {
            return Mathf.Clamp(value, Min, Max);
        }

        public float RandomValue
        {
            get { return UnityEngine.Random.Range(Min, Max); }
        }
    }
}