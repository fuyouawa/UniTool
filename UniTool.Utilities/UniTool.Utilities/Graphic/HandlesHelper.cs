using UnityEditor;
using UnityEngine;

namespace UniTool.Utilities
{
    public static class HandlesHelper
    {
        public static void DrawRectangle(Rect rectangle)
        {
            Handles.DrawSolidRectangleWithOutline(rectangle, Color.white, Color.white);
        }
        public static void DrawRectangle(Vector2 position, Vector2 size)
        {
            Handles.DrawSolidRectangleWithOutline(new Rect(position, size), Color.white, Color.white);
        }
    }
}