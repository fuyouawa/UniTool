using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace UniTool.Editor.Utilities
{
    public abstract class FoldableObjectDrawer<T> : OdinValueDrawer<T>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var options = new FoldoutGroupOptions()
            {
                OnContentGUI = OnContentGUI,
                OnTitleBarGUI = OnTitleBarGUI,
                RightLabel = GetRightLabel(label)
            };
            Property.State.Expanded = UniEditorGUI.FoldoutGroup(GetLabel(label), Property.State.Expanded, Property, options);
        }

        protected virtual string GetLabel(GUIContent label)
        {
            return label.text;
        }

        protected virtual string GetRightLabel(GUIContent label)
        {
            return string.Empty;
        }

        protected virtual void OnTitleBarGUI(Rect rect)
        {
        }

        protected virtual void OnContentGUI()
        {
            foreach (var child in ValueEntry.Property.Children)
            {
                child.Draw();
            }
        }
    }
}
