using UnityEngine;

namespace UniTool.Utilities
{
    public static class VectorExtension
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
    }
}