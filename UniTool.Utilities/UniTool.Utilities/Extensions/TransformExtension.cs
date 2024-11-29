using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniTool.Utilities
{
    public static class TransformExtension
    {
        public static bool IsParentRecursive(this Transform transform, Transform parent)
        {
            if (transform == parent)
                throw new Exception("parent和要判断的transform对象是同一个");

            var p = transform.parent;
            while (p != parent)
            {
                if (p == null)
                    return false;
                p = p.parent;
            }
            return true;
        }

        public static float ScaleSquare(this Transform transform)
        {
            return transform.localScale.magnitude;
        }

        public static void SetScaleSquare(this Transform transform, float size)
        {
            transform.localScale = transform.localScale.normalized * size;
        }

        public static void SetPositionXY(this Transform transform, Vector3 position)
        {
            SetPositionXY(transform, position.ToVec2());
        }

        public static void SetPositionXY(this Transform transform, Vector2 position)
        {
            transform.position = position.ToVec3(transform.position.z);
        }
        
        
        public static string GetRelativePath(this Transform transform, Transform parent, bool includeParent = true)
        {
            if (transform == null)
                return string.Empty;
            var hierarchy = new Stack<string>();

            var p = transform;
            while (p != null && p != parent)
            {
                hierarchy.Push(p.gameObject.name);
                p = p.parent;
            }
            
            if (includeParent && parent != null)
            {
                hierarchy.Push(parent.gameObject.name);
            }

            var path = string.Join("/", hierarchy);

            if (parent == null)
                path = '/' + path;

            return path;
        }

        public static string GetAbsolutePath(this Transform transform, bool includeSceneName = true)
        {
            if (transform == null)
                return string.Empty;
            var path = GetRelativePath(transform, null);

            if (includeSceneName)
                path = '/' + transform.gameObject.scene.name + path;

            return path;
        }
    }
}
