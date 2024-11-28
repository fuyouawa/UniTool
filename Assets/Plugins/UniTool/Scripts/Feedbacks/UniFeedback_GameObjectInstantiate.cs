using Sirenix.OdinInspector;
using UniTool.Tools;
using UniTool.Utilities;
using UnityEngine;

namespace UniTool.Feedbacks
{
    [UniFeedbackHelper("实例化一个GameObject")]
    [AddUniFeedbackMenu("游戏物体/实例化")]
    public class UniFeedback_GameObjectInstantiate : AbstractUniFeedback
    {
        [FoldoutGroup("Instantiate")]
        [Required]
        public GameObject Prefab;
        [FoldoutGroup("Instantiate")]
        public bool HasLiftTime;
        [FoldoutGroup("Instantiate")]
        [ShowIf(nameof(HasLiftTime))]
        public float LiftTime;

        [FoldoutGroup("Transform")]
        public Transform Parent;
        [FoldoutGroup("Transform")]
        public Transform Relative;
        [FoldoutGroup("Transform")]
        public Vector3 Position;
        [FoldoutGroup("Transform")]
        public Vector3 Rotation;
        [FoldoutGroup("Transform")]
        public Vector3 LocalScale = Vector3.one;

        protected override void OnFeedbackPlay()
        {
            var inst = GameObject.Instantiate(Prefab, Parent);
            if (Relative != null)
            {
                inst.transform.position = Relative.transform.position + Position;
                inst.transform.rotation = Relative.transform.rotation * Quaternion.Euler(Rotation);
            }
            else
            {
                inst.transform.SetPositionAndRotation(Position, Quaternion.Euler(Rotation));
            }
            inst.transform.localScale = LocalScale;

            if (HasLiftTime)
            {
                inst.DestroyAfterSeconds(LiftTime);
            }
        }

        protected override void OnFeedbackStop()
        {
            
        }
    }
}
