using UnityEngine;

namespace UniTool.Utilities
{
    public static class CurveExtension
    {
        public static float EvaluateWithRemap(this AnimationCurve curve, float time, float maxTime)
        {
            return curve.Evaluate(MathUtility.Remap(time, 0f, maxTime, 0f, 1f));
        }
        public static float EvaluateWithRemap(this AnimationCurve curve, float time, float maxTime, float minValue, float maxValue)
        {
            return MathUtility.Remap(curve.EvaluateWithRemap(time, maxTime), 0f, 1f, minValue, maxValue);
        }

        public static Color EvaluateWithRemap(this Gradient gradient, float time, float maxTime)
        {
            return gradient.Evaluate(MathUtility.Remap(time, 0f, maxTime, 0f, 1f));
        }
    }
}