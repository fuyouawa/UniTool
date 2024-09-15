using UniTool.Utilities;
using UnityEditor;

namespace UniTool.Editor.Extension
{
    public static class EditorHelper
    {
        public static void ForceRebuildInspectors()
        {
            typeof(EditorUtility).InvokeMethod("ForceRebuildInspectors", null);
        }
    }
}
