using Sirenix.OdinInspector;

namespace UniTool.Feedbacks
{
    [UniFeedbackHelper("运行指定时间(秒)")]
    [AddUniFeedbackMenu("辅助/运行指定时间(秒)")]
    public class UniFeedback_AuxiliaryRunSecond : AbstractUniFeedback
    {
        [FoldoutGroup("Run Second")]
        public float Second;

        public override float GetDuration()
        {
            return Second;
        }

        protected override void OnFeedbackPlay()
        {
            
        }

        protected override void OnFeedbackStop()
        {
            
        }
    }
}
