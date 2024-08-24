#if UNITY_EDITOR
using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UniTool.Feedbacks
{
    [CreateAssetMenu(menuName = "UniTool/Create Feedbacks Config", fileName = "Feedbacks Config")]
    public class FeedbacksConfig : ScriptableObject
    {
        private static FeedbacksConfig _instance;
        public static FeedbacksConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<FeedbacksConfig>("Feedbacks Config");
                }

                if (_instance == null)
                {
                    throw new Exception("UniTool必须要有个Feedbacks Config的资源!");
                }
                return _instance;
            }
        }

        [Title("Language")]
        [OnValueChanged("OnChangeCN")]
        public bool CN;

        private void OnChangeCN()
        {
            Feedbacks.GenerateDropdownItem();
        }
    }
}

#endif
