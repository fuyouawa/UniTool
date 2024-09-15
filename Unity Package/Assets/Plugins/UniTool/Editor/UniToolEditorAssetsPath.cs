namespace UniTool.Editor
{
    public static class UniToolEditorAssetsPath
    {
        public static readonly string ConfigsPath;
        public static readonly string AssetsPath;
        public static readonly string FontsPath;
        public static readonly string IconsPath;

        static UniToolEditorAssetsPath()
        {
            ConfigsPath = UniToolAssetPaths.PluginPath + "/Config/Editor";
            AssetsPath = UniToolAssetPaths.PluginPath + "/Assets/Editor";
            FontsPath = AssetsPath + "/Fonts";
            IconsPath = AssetsPath + "/Icons";
        }
    }
}
