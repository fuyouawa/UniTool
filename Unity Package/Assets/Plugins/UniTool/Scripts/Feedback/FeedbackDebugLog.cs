using Sirenix.OdinInspector;
using UniTool.Attributes;
using UnityEngine;

namespace UniTool.Tools
{
    [FeedbackHelper("打印调试信息")]
    [AddFeedbackMenu("调试/打印")]
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
