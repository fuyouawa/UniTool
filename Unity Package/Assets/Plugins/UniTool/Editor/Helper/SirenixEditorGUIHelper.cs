using System;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace UniTool.Editor.Helper
{
    public static class SirenixEditorGUIHelper
    {
        // public static bool Foldout(Rect labelRect, bool isVisible, GUIContent label, out Rect valueRect,
        //     GUIStyle style = null)
        // {
        //     valueRect = labelRect;
        //     if (label == null)
        //     {
        //         label = new GUIContent(" ");
        //         if (EditorGUIUtility.hierarchyMode)
        //         {
        //             labelRect.width = 2f;
        //         }
        //         else
        //         {
        //             labelRect.width = 18f;
        //             valueRect.xMin += 18f;
        //         }
        //     }
        //     else
        //     {
        //         float indent = GUIHelper.CurrentIndentAmount;
        //         labelRect = new Rect(labelRect.x, labelRect.y, GUIHelper.BetterLabelWidth - indent, labelRect.height);
        //         valueRect.xMin = labelRect.xMax;
        //     }
        //
        //     return SirenixEditorGUI.Foldout(labelRect, isVisible, label);
        // }
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
    }
}
