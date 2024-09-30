using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Utilities.Editor;
using UniTool.Attributes;
using UniTool.Editor.Configs;
using UniTool.Editor.Utilities;
using UnityEditor;
using UnityEngine;

namespace UniTool.Editor.Attributes
{
    public sealed class TitleGroupCNAttributeDrawer : OdinGroupDrawer<TitleGroupCNAttribute>
    {
        public ValueResolver<string> TitleHelper;

        public ValueResolver<string> SubtitleHelper;

        protected override void Initialize()
        {
            TitleHelper = ValueResolver.GetForString(base.Property, base.Attribute.GroupName);
            SubtitleHelper = ValueResolver.GetForString(base.Property, base.Attribute.Subtitle);
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            InspectorProperty property = base.Property;
            TitleGroupCNAttribute attribute = base.Attribute;
            if (property != property.Tree.GetRootProperty(0))
            {
                EditorGUILayout.Space();
            }

            EditorGUIHelper.Title(
                TitleHelper.GetValue(),
                SubtitleHelper.GetValue(),
                (TextAlignment)attribute.Alignment,
                attribute.HorizontalLine,
                attribute.BoldTitle,
                UniToolEditorConfig.Instance.TitleFontSize,
                UniToolEditorConfig.Instance.Font);

            GUIHelper.PushIndentLevel(EditorGUI.indentLevel + (attribute.Indent ? 1 : 0));
            for (int i = 0; i < property.Children.Count; i++)
            {
                InspectorProperty child = property.Children[i];
                child.Draw(child.Label);
            }

            GUIHelper.PopIndentLevel();
        }
    }
}
