using System;
using UnityEngine;

namespace UniTool.Utilities
{
    [CreateAssetMenu(menuName = "UniTool/Create Utilities Config", fileName = "Utilities Config")]
    public class UtilitiesConfig : ScriptableObject
    {
        private static UtilitiesConfig _instance;
        public static UtilitiesConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<UtilitiesConfig>("Utilities Config");
                }

                if (_instance == null)
                {
                    throw new Exception("UniTool.Utilities必须要有个Utilities Config的资源!");
                }
                return _instance;
            }
        }

        public bool CheckAssert = true;
    }
}