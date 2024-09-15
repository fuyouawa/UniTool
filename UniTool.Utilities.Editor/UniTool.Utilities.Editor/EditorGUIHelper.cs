using UnityEditor;
using UnityEngine;

namespace UniTool.Utilities.Editor
{
    public static class EditorGUIHelper
    {
        private static readonly GUIContent _text = new GUIContent();

        public static GUIContent TempContent(string text)
        {
            _text.image = null;
            _text.text = text;
            _text.tooltip = null;
            return _text;
        }

        public static bool HasKeyboardFocus(int controlID)
        {
            return (bool)typeof(EditorGUI).InvokeMethod("HasKeyboardFocus", null, controlID);
        }

        public static void EndEditingActiveTextField()
        {
            typeof(EditorGUI).InvokeMethod("EndEditingActiveTextField", null);
        }
    }
}
