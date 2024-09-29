using UniTool.Tools;
using UnityEditor;
using UnityEngine;

namespace UniTool.Editor.Tools
{
    [CustomPropertyDrawer(typeof(AbstractFeedback))]
    public class AbstractFeedbackDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
        }
    }
}
