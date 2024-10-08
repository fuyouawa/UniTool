using Sirenix.OdinInspector;
using UnityEngine;

namespace UniTool.Tools
{
    [FeedbackHelper("设置GameObject激活状态")]
    [AddFeedbackMenu("游戏物体/设置激活")]
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
