using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;
using System;
using UniTool.Utilities;
using UnityEditor;
using UnityEngine;

namespace UniTool.Editor.Utilities
{
    public static class EditorGUIHelper
    {
        private static readonly GUIContent _text = new GUIContent();

        public static GUIContent TempContent(string text)
        {
            _text.image = null;
            _text.text = text;
            _text.tooltip = null;
            return _text;
        }

        public static bool HasKeyboardFocus(int controlID)
        {
            return (bool)typeof(EditorGUI).InvokeMethod("HasKeyboardFocus", null, controlID);
        }

        public static void EndEditingActiveTextField()
        {
            typeof(EditorGUI).InvokeMethod("EndEditingActiveTextField", null);
        }
        
        public static void Title(string title, string subtitle, TextAlignment textAlignment, bool horizontalLine, bool boldLabel, int fontSize, Font font)
        {
            //IL_0004: Unknown result type (might be due to invalid IL or missing references)
            //IL_0016: Expected I4, but got Unknown
            //IL_007a: Unknown result type (might be due to invalid IL or missing references)
            //IL_007c: Invalid comparison between Unknown and I4
            GUIStyle titleStyle = null;
            GUIStyle subtitleStyle = null;
            switch ((int)textAlignment)
            {
                case 0:
                    titleStyle = (boldLabel ? SirenixGUIStyles.BoldTitle : SirenixGUIStyles.Title);
                    subtitleStyle = SirenixGUIStyles.Subtitle;
                    break;
                case 1:
                    titleStyle = (boldLabel ? SirenixGUIStyles.BoldTitleCentered : SirenixGUIStyles.TitleCentered);
                    subtitleStyle = SirenixGUIStyles.SubtitleCentered;
                    break;
                case 2:
                    titleStyle = (boldLabel ? SirenixGUIStyles.BoldTitleRight : SirenixGUIStyles.TitleRight);
                    subtitleStyle = SirenixGUIStyles.SubtitleRight;
                    break;
                default:
                    titleStyle = (boldLabel ? SirenixGUIStyles.BoldTitle : SirenixGUIStyles.Title);
                    subtitleStyle = SirenixGUIStyles.SubtitleRight;
                    break;
            }

            titleStyle = new GUIStyle(titleStyle)
            {
                font = font,
                fontSize = fontSize
            };
            Rect rect;
            if ((int)textAlignment > 2)
            {
                rect = GUILayoutUtility.GetRect(0f, 18f, titleStyle, GUILayoutOptions.ExpandWidth());
                GUI.Label(rect, title, titleStyle);
                rect.y += 3f;
                GUI.Label(rect, subtitle, subtitleStyle);
                if (horizontalLine)
                {
                    SirenixEditorGUI.HorizontalLineSeparator(SirenixGUIStyles.LightBorderColor);
                    GUILayout.Space(3f);
                }
                return;
            }
            rect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false));
            GUI.Label(rect, title, titleStyle);
            if (subtitle != null && !subtitle.IsNullOrWhitespace())
            {
                rect = EditorGUI.IndentedRect(GUILayoutUtility.GetRect(GUIHelper.TempContent(subtitle), subtitleStyle));
                GUI.Label(rect, subtitle, subtitleStyle);
            }
            if (horizontalLine)
            {
                SirenixEditorGUI.DrawSolidRect(rect.AlignBottom(1f), SirenixGUIStyles.LightBorderColor);
                GUILayout.Space(3f);
            }
        }

        public static void FoldoutProperty(string label, string rightLabel, InspectorProperty property, Action<Rect> onTitleBarGUI, Action onContentGUI)
        {
            SirenixEditorGUI.BeginBox();
            SirenixEditorGUI.BeginBoxHeader();
            var headerRect = EditorGUILayout.GetControlRect(false);

            if (headerRect.position != Vector2.zero)
            {
                if (rightLabel.IsNotNullOrEmpty())
                {
                    var r = GUIHelper.TempContent(rightLabel);
                    var s = SirenixGUIStyles.Label.CalcSize(r);
                    EditorGUI.PrefixLabel(headerRect.AlignRight(s.x), r);
                }

                property.State.Expanded = SirenixEditorGUI.Foldout(headerRect, property.State.Expanded, GUIHelper.TempContent(label));

                onTitleBarGUI?.Invoke(headerRect);
            }

            SirenixEditorGUI.EndBoxHeader();

            if (SirenixEditorGUI.BeginFadeGroup(property, property.State.Expanded))
            {
                onContentGUI?.Invoke();
            }

            SirenixEditorGUI.EndFadeGroup();
            SirenixEditorGUI.EndBox();
        }
    }
}
