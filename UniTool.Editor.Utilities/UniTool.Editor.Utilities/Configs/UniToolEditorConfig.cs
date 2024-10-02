using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace UniTool.Editor.Utilities
{
    public class UniToolEditorConfigAssetPathAttribute : GlobalConfigAttribute
    {
        public UniToolEditorConfigAssetPathAttribute() : base(UniToolEditorAssetsPath.ConfigsPath) {}
    }

    [UniToolEditorConfigAssetPath]
    public class UniToolEditorConfig : GlobalConfig<UniToolEditorConfig>
    {
        public Font Font =>
            AssetDatabase.LoadAssetAtPath<Font>(UniToolEditorAssetsPath.FontsPath+ "/" + FontAssetName);

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
