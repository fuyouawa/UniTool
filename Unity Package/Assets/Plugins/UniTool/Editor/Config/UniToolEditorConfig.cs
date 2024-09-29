using Sirenix.OdinInspector;
using UniTool.Utilities;
using UnityEditor;
using UnityEngine;

namespace UniTool.Editor.Configs
{
    public class UniToolEditorConfigAssetPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public UniToolEditorConfigAssetPathAttribute() : base(UniToolEditorAssetsPath.ConfigsPath) {}
    }

    [UniToolEditorConfigAssetPath]
    public class UniToolEditorConfig : ScriptableObjectSingleton<UniToolEditorConfig>
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
