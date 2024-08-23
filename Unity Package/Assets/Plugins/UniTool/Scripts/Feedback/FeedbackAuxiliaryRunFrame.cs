using System.Collections;
using Sirenix.OdinInspector;

namespace UniTool.Feedbacks
{
    [CustomFeedback("Auxiliary/Run Frame", "运行指定帧数")]
    public class FeedbackAuxiliaryRunFrame : AbstractFeedback
    {
        [FoldoutGroup("Run Frame")]
        public float Frame;

        protected override IEnumerator FeedbackPlayPauser => RunFrameCo();

        private IEnumerator RunFrameCo()
        {
            for (int i = 0; i < Frame; i++)
            {
                yield return null;
            }
        }

        protected override void OnFeedbackPlay()
        {
            
        }

        protected override void OnFeedbackStop()
        {
            
        }
    }
}
