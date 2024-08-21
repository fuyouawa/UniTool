using Sirenix.Utilities.Editor;
using UniTool.Config;
using UnityEngine;

namespace UniTool.Editor.Helper
{
    public static class SirenixGUIStylesHelper
    {
        public static GUIStyle MessageBox
        {
            get
            {
                var config = EditorConfig.GetConfig();
                if (config != null)
                {
                    return config.GetMessageBoxStyle();
                }
                return SirenixGUIStyles.MessageBox;
            }
        }
    }
}
