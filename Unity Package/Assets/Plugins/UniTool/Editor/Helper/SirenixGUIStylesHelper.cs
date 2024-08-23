using UnityEngine;

namespace UniTool.Editor.Helper
{
    public static class SirenixGUIStylesHelper
    {
        public static GUIStyle MessageBox =>
            EditorConfig.Instance.GetMessageBoxStyle();
    }
}
