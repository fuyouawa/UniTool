using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using UniTool.Utilities;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace UniTool.Editor.Utilities
{
    public class FoldoutGroupOptions
    {
        public string RightLabel = null;
        public bool HasBox = true;
        public Action<Rect> OnTitleBarGUI = null;
        public Action OnContentGUI = null;
    }

    public class PopupSelectorOptions
    {
        public Func<object, string> MenuItemNameGetter = null;
        public string Title = null;
        public bool SupportsMultiSelect = false;
    }

    public static class UniEditorGUI
    {
        private static readonly GUIContent _text = new GUIContent();
        private static readonly GUIContent _text2 = new GUIContent();

        public static GUIContent TempContent(string text, string tooltip = null)
        {
            _text.image = null;
            _text.text = text;
            _text.tooltip = tooltip;
            return _text;
        }
        public static GUIContent TempContent(Texture image, string text, string tooltip = null)
        {
            _text.image = image;
            _text.text = text;
            _text.tooltip = tooltip;
            return _text;
        }
        
        public static GUIContent TempContent2(string text, string tooltip = null)
        {
            _text2.image = null;
            _text2.text = text;
            _text2.tooltip = tooltip;
            return _text2;
        }
        public static GUIContent TempContent2(Texture image, string text, string tooltip = null)
        {
            _text2.image = image;
            _text2.text = text;
            _text2.tooltip = tooltip;
            return _text2;
        }

        public static bool HasKeyboardFocus(int controlID)
        {
            return (bool)typeof(EditorGUI).InvokeMethod("HasKeyboardFocus", null, controlID);
        }

        public static void EndEditingActiveTextField()
        {
            typeof(EditorGUI).InvokeMethod("EndEditingActiveTextField", null);
        }

        public static void BigTitle(string title, string subtitle = null,
            TextAlignment textAlignment = TextAlignment.Left, bool horizontalLine = true,
            bool boldLabel = true)
        {
            Title(title, subtitle, textAlignment, horizontalLine, boldLabel,
                UniToolEditorConfig.Instance.BigTitleFontSize, null);
        }

        public static void Title(string title, string subtitle = null, TextAlignment textAlignment = TextAlignment.Left,
            bool horizontalLine = true,
            bool boldLabel = true)
        {
            Title(title, subtitle, textAlignment, horizontalLine, boldLabel, UniToolEditorConfig.Instance.TitleFontSize,
                null);
        }

        public static void Title(string title, string subtitle, TextAlignment textAlignment, bool horizontalLine,
            bool boldLabel, int fontSize, Font font)
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
        public static bool FoldoutGroup(string label, bool expand, object key, FoldoutGroupOptions options = null)
        {
            return FoldoutGroup(EditorGUILayout.GetControlRect(false), label, expand, key, options);
        }

        public static bool FoldoutGroup(Rect headerRect, string label, bool expand, object key, FoldoutGroupOptions options = null)
        {
            options = options ?? new FoldoutGroupOptions();

            if (options.HasBox)
            {
                SirenixEditorGUI.BeginBox();
                SirenixEditorGUI.BeginBoxHeader();
            }

            if (headerRect.position != Vector2.zero)
            {
                if (options.RightLabel.IsNotNullOrEmpty())
                {
                    var r = GUIHelper.TempContent(options.RightLabel);
                    var s = SirenixGUIStyles.Label.CalcSize(r);
                    EditorGUI.PrefixLabel(headerRect.AlignRight(s.x), r);
                }

                expand = SirenixEditorGUI.Foldout(headerRect, expand, GUIHelper.TempContent(label));

                options.OnTitleBarGUI?.Invoke(headerRect);
            }

            if (options.HasBox)
            {
                SirenixEditorGUI.EndBoxHeader();
            }

            if (SirenixEditorGUI.BeginFadeGroup(key, expand))
            {
                options.OnContentGUI?.Invoke();
            }

            SirenixEditorGUI.EndFadeGroup();

            if (options.HasBox)
            {
                SirenixEditorGUI.EndBox();
            }

            return expand;
        }

        public static IEnumerable<T> DrawSelectorDropdown<T>(
            string label,
            string btnLabel,
            IEnumerable<T> collection,
            Action<T> onConfirmed,
            PopupSelectorOptions popupSelectorOptions = null,
            bool returnValuesOnSelectionChange = true,
            GUIStyle style = null,
            params GUILayoutOption[] options)
        {
            return DrawSelectorDropdown(TempContent(label), TempContent2(btnLabel), collection, onConfirmed, popupSelectorOptions,
                returnValuesOnSelectionChange, style, options);
        }

        public static IEnumerable<T> DrawSelectorDropdown<T>(
            GUIContent label,
            GUIContent btnLabel,
            IEnumerable<T> collection,
            Action<T> onConfirmed,
            PopupSelectorOptions popupSelectorOptions = null,
            bool returnValuesOnSelectionChange = true,
            GUIStyle style = null,
            params GUILayoutOption[] options)
        {
            return OdinSelector<T>.DrawSelectorDropdown(label, btnLabel,
                rect => ShowSelectorInPopup(rect, rect.width, collection, onConfirmed, popupSelectorOptions),
                returnValuesOnSelectionChange, style, options);
        }

        private static OdinSelector<T> GetSelector<T>(IEnumerable<T> collection, Action<T> onConfirmed, PopupSelectorOptions options)
        {
            options = options ?? new PopupSelectorOptions();

            GenericSelector<T> selector;
            if (options.MenuItemNameGetter != null)
            {
                selector = new GenericSelector<T>(options.Title, collection, options.SupportsMultiSelect,
                    t => options.MenuItemNameGetter(t));
            }
            else
            {
                selector = new GenericSelector<T>(options.Title, collection, options.SupportsMultiSelect);
            }

            selector.SelectionConfirmed += types =>
            {
                var f = types.FirstOrDefault();
                if (f != null)
                {
                    onConfirmed?.Invoke(f);
                }
            };
            selector.SelectionChanged += types => { selector.SelectionTree.Selection.ConfirmSelection(); };
            return selector;
        }

        public static OdinSelector<T> ShowSelectorInPopup<T>(
            IEnumerable<T> collection,
            Action<T> onConfirmed,
            PopupSelectorOptions options = null)
        {
            var selector = GetSelector(collection, onConfirmed, options);
            selector.ShowInPopup();
            return selector;
        }
        public static OdinSelector<T> ShowSelectorInPopup<T>(
            Rect rect,
            IEnumerable<T> collection,
            Action<T> onConfirmed,
            PopupSelectorOptions options = null)
        {
            var selector = GetSelector(collection, onConfirmed, options);
            selector.ShowInPopup(rect);
            return selector;
        }

        public static OdinSelector<T> ShowSelectorInPopup<T>(
            Rect btnRect,
            float windowWidth,
            IEnumerable<T> collection,
            Action<T> onConfirmed,
            PopupSelectorOptions options = null)
        {
            var selector = GetSelector(collection, onConfirmed, options);
            selector.ShowInPopup(btnRect, windowWidth);
            return selector;
        }

        public static OdinSelector<T> ShowSelectorInPopup<T>(
            float windowWidth,
            IEnumerable<T> collection,
            Action<T> onConfirmed,
            PopupSelectorOptions options = null)
        {
            var selector = GetSelector(collection, onConfirmed, options);
            selector.ShowInPopup(windowWidth);
            return selector;
        }
    }
}
