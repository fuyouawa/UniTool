using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
#endif

namespace UniTool.Utilities
{
    public struct NoLabelVector2
    {
        public float X;
        public float Y;

        public NoLabelVector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Vector2(NoLabelVector2 v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static implicit operator NoLabelVector2(Vector2 v)
        {
            return new NoLabelVector2(v.x, v.y);
        }

        public Vector2 ToVec2()
        {
            return new Vector2(X, Y);
        }
    }

#if UNITY_EDITOR
    public class NoLabelVector2Drawer : OdinValueDrawer<NoLabelVector2>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Rect labelRect;
            Rect contentRect = SirenixEditorGUI.BeginHorizontalPropertyLayout(label, out labelRect);
            EditorGUI.BeginChangeCheck();
            Vector4 val = SirenixEditorFields.VectorPrefixSlideRect(labelRect, base.ValueEntry.SmartValue.ToVec2());
            if (EditorGUI.EndChangeCheck())
            {
                base.ValueEntry.SmartValue = new NoLabelVector2(val.x, val.y);
            }
            GUIHelper.PushLabelWidth(SirenixEditorFields.SingleLetterStructLabelWidth);
            base.Property.Children[0].Draw(null);
            base.Property.Children[1].Draw(null);
            GUIHelper.PopLabelWidth();
            SirenixEditorGUI.EndHorizontalPropertyLayout();
        }
    }
#endif
}
