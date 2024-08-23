using Sirenix.OdinInspector;
using UnityEngine;

namespace UniTool.Feedbacks
{
    [AddFeedbackMenu("GameObject/Instantiate", "实例化一个GameObject")]
    public class FeedbackGameObjectInstantiate : AbstractFeedback
    {
        [FoldoutGroup("Instantiate")]
        [Required]
        public GameObject Prefab;
        [FoldoutGroup("Instantiate")]
        public bool HasLiftTime;
        [FoldoutGroup("Instantiate")]
        [ShowIf("HasLiftTime")]
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
                inst.AddComponent<LifeTime>().Run(LiftTime);
            }
        }

        protected override void OnFeedbackStop()
        {
            
        }
    }
}