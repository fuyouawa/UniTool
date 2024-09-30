using UniTool.Utilities;
using UnityEditor;

namespace UniTool.Editor.Utilities
{
    public static class EditorHelper
    {
        public static void ForceRebuildInspectors()
        {
            typeof(EditorUtility).InvokeMethod("ForceRebuildInspectors", null);
        }
    }
}
