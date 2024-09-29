namespace UniTool.Editor
{
    public static class UniToolEditorAssetsPath
    {
        public static readonly string ResPath;
        public static readonly string ConfigsPath;
        public static readonly string FontsPath;

        static UniToolEditorAssetsPath()
        {
            ResPath = UniToolAssetPaths.PluginPath + "/Editor/Resources";
            ConfigsPath = ResPath + "/Config";
            FontsPath = ResPath + "/Fonts";
        }
    }
}
