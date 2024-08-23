using Sirenix.OdinInspector;

namespace UniTool.Feedbacks
{
    [FeedbackHelper("运行指定时间(秒)")]
    [AddFeedbackMenu("Auxiliary/Run Second")]
    public class FeedbackAuxiliaryRunSecond : AbstractFeedback
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
