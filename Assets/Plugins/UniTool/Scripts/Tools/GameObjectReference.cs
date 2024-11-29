using System;
using System.Collections.Generic;
using UniTool.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UniTool.Tools
{
    [Serializable]
    public struct GameObjectReference
    {
        [SerializeField]
        private string _absolutePath;

        public string AbsolutePath => _absolutePath ?? string.Empty;

        public GameObjectReference(GameObject gameObject)
        {
            _absolutePath = gameObject?.transform.GetAbsolutePath();
        }

        public GameObject DeRef()
        {
            return GameObjectHelper.FindByAbsolutePath(_absolutePath);
        }
    }
}
