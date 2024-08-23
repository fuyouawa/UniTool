using Sirenix.OdinInspector;
using UniTool.PropertyPicker;

namespace UniTool.Feedbacks
{
    [FeedbackHelper("调用GameObject上的一个函数")]
    [AddFeedbackMenu("GameObject/Invoke Method")]
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
