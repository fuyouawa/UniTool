using Sirenix.Utilities.Editor;
using UnityEngine;

namespace UniTool.Editor.Helper
{
    public static class SirenixGUIStylesHelper
    {
        public static GUIStyle InfoBoxCN =>
            EditorConfig.Instance.InfoBoxCNStyle.ToGUI(SirenixGUIStyles.MessageBox);
        public static GUIStyle LabelTextCN =>
            EditorConfig.Instance.LabelTextCNStyle.ToGUI(SirenixGUIStyles.Label);
    }
}
