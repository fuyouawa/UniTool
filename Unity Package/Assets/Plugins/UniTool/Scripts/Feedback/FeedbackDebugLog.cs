using Sirenix.OdinInspector;
using UnityEngine;

namespace UniTool.Feedbacks
{
    [FeedbackHelper("打印调试信息")]
    [AddFeedbackMenu("Debug/Log")]
    public class FeedbackDebugLog : AbstractFeedback
    {
        [FoldoutGroup("Log")]
        public string Message;


        protected override void OnFeedbackPlay()
        {
            Debug.Log(Message);
        }

        protected override void OnFeedbackStop()
        {

        }
    }
}
