using Sirenix.Utilities.Editor;
using UniTool.Editor.Configs;
using UnityEngine;

namespace UniTool.Editor.Helper
{
    public static class GUIStylesHelper
    {
        public static GUIStyle InfoBoxCN => new(SirenixGUIStyles.MessageBox)
        {
            font = UniToolEditorConfig.Instance.Font,
            fontSize = UniToolEditorConfig.Instance.InfoBoxFontSize,
            margin = UniToolEditorConfig.Instance.InfoBoxMargin,
            padding = UniToolEditorConfig.Instance.InfoBoxPadding
        };
    }
}
