using Sirenix.OdinInspector.Editor;
using UniTool.Feedbacks;
using UnityEditor;

namespace UniTool.Editor.Feedbacks
{
    [CustomEditor(typeof(FeedbacksManager))]
    public class FeedbacksManagerEditor : OdinEditor
    {
        private void OnSceneGUI()
        {
            (target as FeedbacksManager)?.OnSceneGUI();
        }
    }
}
