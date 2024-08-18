using UnityEngine;

namespace UniTool.Extension
{
    public static class GameObjectExtension
    {
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            if (!obj.TryGetComponent(out T component))
            {
                component = obj.AddComponent<T>();
            }

            return component;
        }
    }
}