using UnityEngine;

namespace UniTool.Utilities
{
    public static class VectorHelper
    {
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