using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System;
using UnityEngine;

namespace UniTool.Editor
{
    [CreateAssetMenu(menuName = "UniTool/Create Editor Config", fileName = "Editor Config")]
    public class EditorConfig : ScriptableObject
    {
        private static EditorConfig _instance;
        public static EditorConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<EditorConfig>("Editor Config");
                }

                if (_instance == null)
                {
                    throw new Exception("UniTool必须要有个Editor Config的资源!");
                }
                return _instance;
            }
        }


        // [MenuItem("Tools/UniTool/Create Editor Config")]
        // private static void CreateEditorConfig()
        // {
        //     var config = CreateInstance<EditorConfig>();
        //
        //     string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        //     if (string.IsNullOrEmpty(path))
        //     {
        //         path = "Assets";
        //     }
        //     else if (Path.GetExtension(path) != "")
        //     {
        //         path = path.Replace(Path.GetFileName(path), "");
        //     }
        //
        //     if (!Directory.Exists(path))
        //     {
        //         path = "Assets";
        //     }
        //
        //     var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/Editor Config.asset");
        //
        //     // 将实例保存为资产
        //     AssetDatabase.CreateAsset(config, assetPathAndName);
        //     AssetDatabase.SaveAssets();
        //     AssetDatabase.Refresh();
        //
        //     // 选中新创建的资产
        //     EditorUtility.FocusProjectWindow();
        //     Selection.activeObject = config;
        // }
        [Title("MessageBox Style")]
        [LabelText("Font")]
        public Font MessageBoxFont;
        [LabelText("Font Size")]
        public int MessageBoxFontSize = 12;
        [LabelText("Margin")]
        public RectOffset MessageBoxMargin;
        [LabelText("Padding")]
        public RectOffset MessageBoxPadding;

        public GUIStyle GetMessageBoxStyle()
        {
            return new GUIStyle(SirenixGUIStyles.MessageBox)
            {
                font = MessageBoxFont,
                fontSize = MessageBoxFontSize,
                margin = MessageBoxMargin,
                padding = MessageBoxPadding
            };
        }
    }
}
