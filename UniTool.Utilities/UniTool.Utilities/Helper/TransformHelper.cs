using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UniTool.Utilities
{
    public class TransformRecorder
    {
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
        public Vector3 LocalScale;
        public Transform Parent;
        public Transform Target;

        TransformRecorder(Transform target)
        {
            Reset(target);
        }

        void Reset(Transform target)
        {
            LocalPosition = target.localPosition;
            LocalRotation = target.localRotation;
            LocalScale = target.localScale;
            Parent = target.parent;
            Target = target;
        }

        void Apply()
        {
            Target.SetParent(Parent);
            Target.localPosition = LocalPosition;
            Target.localRotation = LocalRotation;
            Target.localScale = LocalScale;
        }
    }

    public static class TransformHelper
    {
    }
}
