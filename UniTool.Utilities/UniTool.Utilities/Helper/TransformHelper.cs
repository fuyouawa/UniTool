using UnityEngine;

namespace UniTool.Utilities
{
    public class TransformLocalInfo
    {
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
        public Vector3 LocalScale;
        public Transform Parent;
    }

    public static class TransformHelper
    {
        public static TransformLocalInfo GetLocalInfo(Transform transform)
        {
            return new TransformLocalInfo()
            {
                LocalPosition = transform.localPosition,
                LocalRotation = transform.localRotation,
                LocalScale = transform.localScale,
                Parent = transform.parent
            };
        }

        public static void SetLocalInfo(Transform transform, TransformLocalInfo info)
        {
            transform.SetParent(info.Parent);
            transform.localPosition = info.LocalPosition;
            transform.localRotation = info.LocalRotation;
            transform.localScale = info.LocalScale;
        }
    }
}