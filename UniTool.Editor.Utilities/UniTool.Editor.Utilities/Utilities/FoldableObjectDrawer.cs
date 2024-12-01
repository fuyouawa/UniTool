using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace UniTool.Editor.Utilities
{
    public abstract class FoldableObjectDrawer<T> : OdinValueDrawer<T>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var config = new FoldoutHeaderConfig(GetLabel(label))
            {
                Expand = Property.State.Expanded,
                RightLabel = new GUIContent(GetRightLabel(label))
            };

            SirenixEditorGUI.BeginBox();
            Property.State.Expanded = UniEditorGUI.BeginFoldoutHeader(config, out var headerRect);
            OnTitleBarGUI(headerRect);
            UniEditorGUI.EndFoldoutHeader();

            if (SirenixEditorGUI.BeginFadeGroup(UniqueDrawerKey.Create(Property, this), Property.State.Expanded))
            {
                OnContentGUI();
            }
            SirenixEditorGUI.EndFadeGroup();
            SirenixEditorGUI.EndBox();
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
