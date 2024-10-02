namespace UniTool.Editor.Utilities
{
    public static class UniToolEditorAssetsPath
    {
        public static readonly string ResPath;
        public static readonly string ConfigsPath;
        public static readonly string FontsPath;

        static UniToolEditorAssetsPath()
        {
            ResPath = "Assets/Plugins/UniTool/Editor/Resources";
            ConfigsPath = ResPath + "/Config";
            FontsPath = ResPath + "/Fonts";
        }
    }
}
