using System;
using UnityEngine;

namespace UniTool.Helper
{
    public static class VectorHelper
    {
        public static Vector3 ToVec3(this Vector2 v)
        {
            return new Vector3(v.x, v.y);
        }
        public static Vector2 ToVec2(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector2 Rotate(this Vector2 v, float angle)
        {
            float rad = angle * Mathf.Deg2Rad; // 将角度转换为弧度
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            // 计算旋转后的新向量
            float newX = v.x * cos - v.y * sin;
            float newY = v.x * sin + v.y * cos;

            return new Vector2(newX, newY);
        }

        public static Vector2 ToCrossDirection(Vector2 v)
        {
            v = v.normalized;

            var absX = Mathf.Abs(v.x);
            var absY = Mathf.Abs(v.y);

            if (v.x > 0f)
            {
                if (v.y > 0f)
                {
                    return v.x > v.y ? Vector2.right : Vector2.up;
                }
                else
                {
                    return v.x > absY ? Vector2.right : Vector2.down;
                }
            }
            else
            {
                if (v.y > 0f)
                {
                    return absX > v.y ? Vector2.left : Vector2.up;
                }
                else
                {
                    return absX > absY ? Vector2.left : Vector2.down;
                }
            }
        }

        public static bool IsVertical(Vector2 v)
        {
            v = ToCrossDirection(v);
            return v == Vector2.up || v == Vector2.down;
        }
    }
}