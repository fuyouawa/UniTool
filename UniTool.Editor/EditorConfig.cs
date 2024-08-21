using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace UniTool.Editor
{
    [CreateAssetMenu(menuName = "UniTool/Create Editor Config", fileName = "Editor Config")]
    public class EditorConfig : ScriptableObject
    {
        [MenuItem("Tools/UniTool/Create Editor Config")]
        private static void CreateEditorConfig()
        {
            var config = CreateInstance<EditorConfig>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(path), "");
            }

            if (!Directory.Exists(path))
            {
                path = "Assets";
            }

            var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/Editor Config.asset");

            // 将实例保存为资产
            AssetDatabase.CreateAsset(config, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // 选中新创建的资产
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = config;
        }

        [Title("MessageBox Style")]
        [LabelText("Font")]
        public Font MessageBoxFont;
        [LabelText("Font Size")]
        public int MessageBoxFontSize = 12;
        [LabelText("Margin")]
        public RectOffset MessageBoxMargin;

        private static EditorConfig _config;

        public static EditorConfig GetConfig()
        {
            if (_config == null)
            {
                _config = Resources.Load<EditorConfig>("Editor Config");
            }
            return _config;
        }

        public GUIStyle GetMessageBoxStyle()
        {
            return new GUIStyle(SirenixGUIStyles.MessageBox)
            {
                font = MessageBoxFont,
                fontSize = MessageBoxFontSize,
                margin = MessageBoxMargin
            };
        }
    }
}
