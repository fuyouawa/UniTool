using Sirenix.OdinInspector;
using UniTool.Utilities;
using UnityEngine;

namespace UniTool.Feedbacks
{
    public class LifeTime : MonoBehaviour
    {
        public bool Enable = true;
        public float DestroyLifeTime = 1f;
        [ShowInInspector]
        public float RemainingTime { get; private set; }

        private void Start()
        {
            RemainingTime = DestroyLifeTime;
        }
        private void Update()
        {
            if (Enable)
            {
                if (RemainingTime <= 0)
                {
                    Destroy(gameObject);
                }
                else
                {
                    RemainingTime -= Time.deltaTime;
                }
            }
        }

        public void Run(float lifeTime)
        {
            Enable = true;
            DestroyLifeTime = lifeTime;
        }
    }
}
