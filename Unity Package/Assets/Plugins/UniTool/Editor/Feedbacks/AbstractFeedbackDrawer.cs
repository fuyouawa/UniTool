using UniTool.Feedbacks;
using UnityEditor;
using UnityEngine;

namespace UniTool.Editor.Feedbacks
{
    [CustomPropertyDrawer(typeof(AbstractFeedback))]
    public class AbstractFeedbackDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
        }
    }
}
