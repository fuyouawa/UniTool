using Sirenix.OdinInspector;
using UnityEngine;

namespace UniTool.Feedbacks
{
    [UniFeedbackHelper("设置GameObject激活状态")]
    [AddUniFeedbackMenu("游戏物体/设置激活")]
    public class UniFeedback_GameObjectSetActive : AbstractUniFeedback
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
