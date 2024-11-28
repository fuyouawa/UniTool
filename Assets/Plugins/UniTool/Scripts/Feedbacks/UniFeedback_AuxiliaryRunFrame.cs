using System.Collections;
using Sirenix.OdinInspector;

namespace UniTool.Feedbacks
{
    [UniFeedbackHelper("运行指定帧数")]
    [AddUniFeedbackMenu("辅助/运行指定帧数")]
    public class UniFeedback_AuxiliaryRunFrame : AbstractUniFeedback
    {
        [FoldoutGroup("Run Frame")]
        public float Frame;

        protected override IEnumerator Pause => RunFrameCo();

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
