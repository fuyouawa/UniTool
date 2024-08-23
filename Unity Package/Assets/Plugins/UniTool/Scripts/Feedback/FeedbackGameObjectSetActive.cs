using Sirenix.OdinInspector;
using UnityEngine;

namespace UniTool.Feedbacks
{
    [FeedbackHelper("设置GameObject激活状态")]
    [AddFeedbackMenu("GameObject/Set Active")]
    public class FeedbackGameObjectSetActive : AbstractFeedback
    {
        [FoldoutGroup("Set Active")]
        public GameObject BoundObject;
        [FoldoutGroup("Set Active")]
        public bool ActiveToSet = true;

        protected override void OnFeedbackPlay()
        {
            BoundObject.SetActive(ActiveToSet);
        }

        protected override void OnFeedbackStop()
        {
        }
    }
}
