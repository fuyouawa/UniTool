using Sirenix.OdinInspector;

namespace UniTool.Feedbacks
{
    [CustomFeedback("Auxiliary/Run Second", "运行指定时间(秒)")]
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
