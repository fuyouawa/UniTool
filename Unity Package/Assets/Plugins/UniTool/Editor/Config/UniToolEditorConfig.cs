using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System;
using System.IO;
using Sirenix.Utilities;
using UniTool.Global;
using UnityEditor;
using UnityEngine;

namespace UniTool.Editor.Configs
{
    [UniToolEditorGlobalConfig]
    public class UniToolEditorConfig : GlobalConfig<UniToolEditorConfig>
    {
        private Font _font;

        public Font Font
        {
            get
            {
                if (_font == null)
                {
                    _font = AssetDatabase.LoadAssetAtPath<Font>(Path.Combine(UniToolAssetPaths.EditorAssetsPath, FontAssetName));
                }
                return _font;
            }
        }

        public string FontAssetName;

        [FoldoutGroup("InfoBox Style")]
        [LabelText("Font Size")]
        public int InfoBoxFontSize;
        [FoldoutGroup("InfoBox Style")]
        [LabelText("Margin")]
        public RectOffset InfoBoxMargin;
        [FoldoutGroup("InfoBox Style")]
        [LabelText("Padding")]
        public RectOffset InfoBoxPadding;

        [FoldoutGroup("Title Style")]
        public int TitleFontSize;
        [FoldoutGroup("Title Style")]
        public int BigTitleFontSize;
    }
}
