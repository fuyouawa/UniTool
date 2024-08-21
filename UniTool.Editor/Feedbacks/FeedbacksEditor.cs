using Sirenix.OdinInspector.Editor;
using UniTool.Feedbacks;
using UnityEditor;

namespace UniTool.Editor.Feedbacks
{
    [CustomEditor(typeof(UniTool.Feedbacks.Feedbacks))]
    public class FeedbacksEditor : OdinEditor
    {
        private void OnSceneGUI()
        {
            (target as UniTool.Feedbacks.Feedbacks)?.OnSceneGUI();
        }
    }
}
