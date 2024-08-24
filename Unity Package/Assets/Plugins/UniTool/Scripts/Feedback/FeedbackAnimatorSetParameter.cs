using Sirenix.OdinInspector;
using System;
using UniTool.Utilities;
using UnityEngine;

namespace UniTool.Feedbacks
{
    [FeedbackHelper("设置指定Animator的参数")]
    [AddFeedbackMenu("Animator/Set Parameter")]
    [AddFeedbackMenuCN("动画器/设置参数")]
    public class FeedbackAnimatorSetParameter : AbstractFeedback
    {
        public enum ParameterTypes
        {
            Int,
            Float,
            Bool
        }

        [FoldoutGroup("Set Parameter")]
        [Required]
        public Animator TargetAnimator;
        [FoldoutGroup("Set Parameter")]
        public string ParameterName;
        [FoldoutGroup("Set Parameter")]
        public ParameterTypes ParameterType;
        [FoldoutGroup("Set Parameter")]
        [ShowIf("ParameterType", ParameterTypes.Int)]
        public int IntToSet;
        [FoldoutGroup("Set Parameter")]
        [ShowIf("ParameterType", ParameterTypes.Float)]
        public float FloatToSet;
        [FoldoutGroup("Set Parameter")]
        [ShowIf("ParameterType", ParameterTypes.Bool)]
        public bool BoolToSet;

        private int _paramId;

        protected override void OnFeedbackInit()
        {
            if (!TargetAnimator.HasParam(ParameterName, AnimatorControllerParameterType.Trigger))
                throw new Exception($"Animator({TargetAnimator}) has no trigger parameter({ParameterName})");
            _paramId = Animator.StringToHash(ParameterName);
        }

        protected override void OnFeedbackPlay()
        {
            switch (ParameterType)
            {
                case ParameterTypes.Int:
                    TargetAnimator.SetInteger(_paramId, IntToSet);
                    break;
                case ParameterTypes.Float:
                    TargetAnimator.SetFloat(_paramId, FloatToSet);
                    break;
                case ParameterTypes.Bool:
                    TargetAnimator.SetBool(_paramId, BoolToSet);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnFeedbackStop()
        {
        }
    }
}