using System.Linq;
using UniTool.Editor.Utilities;
using UniTool.Tools;
using UniTool.Utilities;
using UnityEditor;
using UnityEngine;

namespace UniTool.Editor.Tools
{
    public class AbstractFeedbackDrawer : FoldableObjectDrawer<AbstractFeedback>
    {
        private static readonly float IconWidth = EditorGUIUtility.singleLineHeight;

        protected override void OnTitleBarGUI(Rect rect)
        {
            base.OnTitleBarGUI(rect);

            var value = ValueEntry.SmartValue;

            var buttonRect = new Rect(rect)
            {
                x = rect.x + 17,
                width = IconWidth,
                height = IconWidth
            };

            value.Enable = EditorGUI.Toggle(buttonRect, value.Enable);
        }

        protected override string GetRightLabel(GUIContent label)
        {
            var attr = ValueEntry.SmartValue.GetType().GetCustomAttribute<AddFeedbackMenuAttribute>();
            if (attr != null)
            {
                return $"[{attr.Path.Split("/").Last()}]";
            }
            return base.GetRightLabel(label);
        }

        protected override string GetLabel(GUIContent label)
        {
            var value = ValueEntry.SmartValue;

            return "      " + (value.Label.IsNullOrEmpty() ? "TODO" : value.Label);
        }
    }
}
