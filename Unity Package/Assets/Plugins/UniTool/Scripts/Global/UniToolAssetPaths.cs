using System.IO;

namespace UniTool.Global
{
    public static class UniToolAssetPaths
    {
        public static readonly string PluginPath;
        public static readonly string EditorConfigsPath;
        public static readonly string EditorAssetsPath;

        static UniToolAssetPaths()
        {
            PluginPath = "Assets/Plugins/UniTool";
            EditorConfigsPath = Path.Combine(PluginPath, "Config/Editor");
            EditorAssetsPath = Path.Combine(PluginPath, "Config/Editor");
        }
    }
}
