using System;
using System.Collections;
using Sirenix.OdinInspector;
using UniTool.Utilities;
using UnityEngine;

namespace UniTool.Feedbacks
{
    [UniFeedbackHelper("触发指定Animator的Trigger参数")]
    [AddUniFeedbackMenu("动画器/触发参数")]
    public class UniFeedback_AnimatorTriggerParameter : AbstractUniFeedback
    {
        [FoldoutGroup("Trigger Param")]
        [Required]
        public Animator TargetAnimator;

        [FoldoutGroup("Trigger Param")]
        [Tooltip("要进行触发的Trigger参数名称")]
        public string ParameterName;

        private int _paramId;

        protected override void OnFeedbackInit()
        {
            _paramId = Animator.StringToHash(ParameterName);
        }

        protected override void OnFeedbackPlay()
        {
            TargetAnimator.SetTrigger(_paramId);
        }

        protected override void OnFeedbackStop()
        {
        }
    }
}
