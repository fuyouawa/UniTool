using System;
using Sirenix.OdinInspector;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace UniTool.Feedbacks
{
    [CustomFeedback("GameObject/Set Active", "设置GameObject激活状态")]
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
