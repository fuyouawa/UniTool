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

        public static string GetRelativePath(GameObject child, GameObject parent)
        {
            if (child == parent)
            {
                return "";
            }

            string path = child.name;
            Transform current = child.transform;

            while (current.parent != null && current.parent.gameObject != parent)
            {
                current = current.parent;
                path = current.name + "/" + path; // 构建路径
            }

            if (current.parent == null)
            {
                Debug.LogError("The specified parent is not an ancestor of the child object.");
                return null;
            }

            return path;
        }
    }
}