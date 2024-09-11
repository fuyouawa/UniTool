﻿using System;
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
    }
}