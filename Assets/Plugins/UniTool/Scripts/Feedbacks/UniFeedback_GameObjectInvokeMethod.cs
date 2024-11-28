using Sirenix.OdinInspector;
using UniTool.Tools;

namespace UniTool.Feedbacks
{
    [UniFeedbackHelper("调用GameObject上的一个函数")]
    [AddUniFeedbackMenu("游戏物体/调用函数")]
    public class UniFeedback_GameObjectInvokeMethod : AbstractUniFeedback
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
