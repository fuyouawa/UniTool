using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace UniTool.Editor.Window
{
    internal class AssetReferenceSearcherWindow : OdinMenuEditorWindow
    {
        private static AssetReferenceSearcherWindow _instance;

        [MenuItem("Tools/UniTool/资源引用查找器")]
        public static void ShowWindow()
        {
            _instance = GetWindow<AssetReferenceSearcherWindow>("资源引用查找器");
        }

        private ComponentSearcher _componentSearcher = new ComponentSearcher();

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree()
            {
                { "组件查找", _componentSearcher }
            };
            tree.Selection.SupportsMultiSelect = false;
            return tree;
        }
    }
}
