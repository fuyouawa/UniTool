using Sirenix.OdinInspector;
using UniTool.Utilities;

namespace UniTool.Tools
{
    [FeedbackHelper("调用GameObject上的一个函数")]
    [AddFeedbackMenu("游戏物体/调用函数")]
    public class FeedbackGameObjectInvokeMethod : AbstractFeedback
    {
        [Required]
        [FoldoutGroup("Invoke Method")]
        [HideReferenceObjectPicker]
        [HideLabel]
        [DisableInPlayMode]
        public MethodPicker Picker = new();
        
        protected override void OnFeedbackPlay()
        {
            Picker.TryInvoke(out _);
        }

        protected override void OnFeedbackStop()
        {
        }
    }
}
