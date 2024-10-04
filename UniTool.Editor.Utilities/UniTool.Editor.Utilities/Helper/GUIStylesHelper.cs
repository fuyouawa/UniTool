using Sirenix.Utilities.Editor;
using UnityEngine;

namespace UniTool.Editor.Utilities
{
    public static class GUIStylesHelper
    {
        public static GUIStyle InfoBoxCN => new GUIStyle(SirenixGUIStyles.MessageBox)
        {
            fontSize = UniToolEditorConfig.Instance.InfoBoxFontSize,
            margin = UniToolEditorConfig.Instance.InfoBoxMargin,
            padding = UniToolEditorConfig.Instance.InfoBoxPadding,
            fontStyle = UniToolEditorConfig.Instance.InfoBoxFontStyle
        };
    }
}
